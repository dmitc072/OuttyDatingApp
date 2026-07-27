using Android.App;
using Android.Content;
using Android.Content.PM;

namespace Outty.Mobile;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
    DataScheme = CallbackScheme)]
public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
{
    // Must match GoogleAuthOptions.RedirectUri's scheme exactly (the "iOS URL scheme"
    // shown on the iOS-type OAuth client in Google Cloud Console).
    public const string CallbackScheme = "com.googleusercontent.apps.757401812518-f0uq2e65clud7o8ou51uumi9jl2896de";
}
