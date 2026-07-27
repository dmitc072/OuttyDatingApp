using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Outty.Mobile.Services;

public class GoogleAuthService
{
    private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";

    public async Task<string> SignInAndGetIdTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(GoogleAuthOptions.ClientId))
        {
            throw new InvalidOperationException(
                "Google OAuth Client ID is not configured. Set GoogleAuthOptions.ClientId " +
                "to your own Google Cloud Console OAuth client (see docs/setup.md).");
        }

        var codeVerifier = GeneratePkceCodeVerifier();
        var codeChallenge = GeneratePkceCodeChallenge(codeVerifier);

        var authorizeUrl =
            $"{AuthorizationEndpoint}" +
            $"?client_id={Uri.EscapeDataString(GoogleAuthOptions.ClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(GoogleAuthOptions.RedirectUri)}" +
            "&response_type=code" +
            "&scope=openid%20email%20profile" +
            $"&code_challenge={codeChallenge}" +
            "&code_challenge_method=S256";

        var authResult = await WebAuthenticator.Default.AuthenticateAsync(
            new WebAuthenticatorOptions
            {
                Url = new Uri(authorizeUrl),
                CallbackUrl = new Uri(GoogleAuthOptions.RedirectUri)
            });

        if (!authResult.Properties.TryGetValue("code", out var authorizationCode) ||
            string.IsNullOrWhiteSpace(authorizationCode))
        {
            throw new InvalidOperationException("Google did not return an authorization code.");
        }

        return await ExchangeCodeForIdTokenAsync(authorizationCode, codeVerifier);
    }

    private static async Task<string> ExchangeCodeForIdTokenAsync(string code, string codeVerifier)
    {
        using var httpClient = new HttpClient();

        var tokenRequestParameters = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = GoogleAuthOptions.ClientId,
            ["redirect_uri"] = GoogleAuthOptions.RedirectUri,
            ["grant_type"] = "authorization_code",
            ["code_verifier"] = codeVerifier
        };

        if (!string.IsNullOrWhiteSpace(GoogleAuthOptions.ClientSecret))
        {
            tokenRequestParameters["client_secret"] = GoogleAuthOptions.ClientSecret;
        }

        using var response = await httpClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(tokenRequestParameters));

        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        return tokenResponse?.IdToken
            ?? throw new InvalidOperationException("Google did not return an id_token.");
    }

    private static string GeneratePkceCodeVerifier()
    {
        return Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
    }

    private static string GeneratePkceCodeChallenge(string codeVerifier)
    {
        var challengeBytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(challengeBytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private class GoogleTokenResponse
    {
        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
    }
}
