using Outty.Mobile.Services;

namespace Outty.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly GoogleAuthService _googleAuthService;
    private readonly ApiClient _apiClient;

    public LoginPage(GoogleAuthService googleAuthService, ApiClient apiClient)
    {
        InitializeComponent();

        _googleAuthService = googleAuthService;
        _apiClient = apiClient;
    }

    private async void OnGoogleLoginClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            SetLoadingState(true);

            var idToken = await _googleAuthService.SignInAndGetIdTokenAsync();

            var loginResult = await _apiClient.LoginWithGoogleAsync(idToken);

            Preferences.Default.Set("UserId", loginResult.UserId);
            Preferences.Default.Set("UserEmail", loginResult.Email);

            await Shell.Current.GoToAsync(
                loginResult.HasProfile
                    ? $"///{nameof(HomePage)}"
                    : nameof(CreateProfilePage));
        }
        catch (Exception ex)
        {
            ErrorLabel.Text =
                $"Unable to sign in: {ex.Message}";

            ErrorLabel.IsVisible = true;
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        GoogleLoginButton.IsEnabled = !isLoading;

        LoginActivityIndicator.IsVisible = isLoading;
        LoginActivityIndicator.IsRunning = isLoading;

        if (isLoading)
        {
            ErrorLabel.IsVisible = false;
            ErrorLabel.Text = string.Empty;
        }
    }
}