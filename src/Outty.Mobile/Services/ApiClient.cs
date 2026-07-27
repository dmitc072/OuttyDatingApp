using System.Net.Http.Json;

namespace Outty.Mobile.Services;

public record GoogleLoginResult(int UserId, string Email, bool HasProfile);

public record InterestSelection(string Name, string ExperienceLevel);

public record CreateProfileRequest(
    int UserId,
    string DisplayName,
    DateOnly BirthDate,
    string City,
    string State,
    string ZipCode,
    string? Pronouns,
    string? Bio,
    string PreferredDistance,
    int SearchRadiusMiles,
    List<InterestSelection> Interests,
    List<string> Goals);

public record CreateProfileResult(int Id);

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiOptions.BaseAddress)
        };
    }

    public async Task<GoogleLoginResult> LoginWithGoogleAsync(string idToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/users/login",
            new { IdToken = idToken });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GoogleLoginResult>()
            ?? throw new InvalidOperationException("Login response was empty.");
    }

    public async Task<CreateProfileResult> CreateProfileAsync(CreateProfileRequest request)
    {
        using var response = await _httpClient.PostAsJsonAsync("/profiles", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Unable to create profile ({(int)response.StatusCode}): {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<CreateProfileResult>()
            ?? throw new InvalidOperationException("Create profile response was empty.");
    }
}
