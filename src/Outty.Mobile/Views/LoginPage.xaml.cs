namespace Outty.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnGoogleLoginClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            SetLoadingState(true);

            // Simulated Google Sign-In for the class project.
            await Shell.Current.GoToAsync(
                nameof(CreateProfilePage));
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