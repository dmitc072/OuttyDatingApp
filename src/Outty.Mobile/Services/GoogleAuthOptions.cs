namespace Outty.Mobile.Services;


public static class GoogleAuthOptions
{
    // iOS-type OAuth client (required by Google for custom URI scheme redirects).
    public const string ClientId = "757401812518-f0uq2e65clud7o8ou51uumi9jl2896de.apps.googleusercontent.com";

    // iOS/Android-type clients are public clients — Google does not issue a secret.
    public const string ClientSecret = "";

    // Exact "iOS URL scheme" value from Google Cloud Console for this client.
    // Must match WebAuthenticationCallbackActivity.CallbackScheme (Android) exactly.
    public const string RedirectUri = "com.googleusercontent.apps.757401812518-f0uq2e65clud7o8ou51uumi9jl2896de:/oauthredirect";
}
