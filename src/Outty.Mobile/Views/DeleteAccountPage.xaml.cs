namespace Outty.Mobile.Views;

public partial class DeleteAccountPage : ContentPage
{
    public DeleteAccountPage()
    {
        InitializeComponent();
    }

    private void OnConfirmationTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        ClearError();

        var confirmationText =
            e.NewTextValue?.Trim() ?? string.Empty;

        DeleteAccountButton.IsEnabled =
            confirmationText.Equals(
                "DELETE",
                StringComparison.OrdinalIgnoreCase);

        DeleteAccountButton.Opacity =
            DeleteAccountButton.IsEnabled ? 1.0 : 0.5;
    }

    private async void OnDeleteAccountClicked(
        object? sender,
        EventArgs e)
    {
        ClearError();

        var confirmationText =
            ConfirmationEntry.Text?.Trim() ?? string.Empty;

        if (!confirmationText.Equals(
                "DELETE",
                StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Please type DELETE to continue.");
            return;
        }

        var confirmed =
            await DisplayAlertAsync(
                "Delete Account?",
                "Are you sure you want to permanently delete your Outty account? This action cannot be undone.",
                "Delete",
                "Cancel");

        if (!confirmed)
        {
            return;
        }

        DeleteAccountButton.IsEnabled = false;
        DeleteAccountButton.Opacity = 0.5;
        DeleteAccountButton.Text = "Deleting Account...";

        try
        {
            DeleteLocalProfileData();

            await DisplayAlertAsync(
                "Account Deleted",
                "Your Outty account has been deleted.",
                "OK");

            ConfirmationEntry.Text = string.Empty;

            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            ShowError(
                $"Unable to delete the account: {ex.Message}");

            DeleteAccountButton.Text = "Delete My Account";
            DeleteAccountButton.IsEnabled = true;
            DeleteAccountButton.Opacity = 1.0;
        }
    }

    private async void OnCancelClicked(
        object? sender,
        EventArgs e)
    {
        ClearError();

        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        await Shell.Current.GoToAsync("//HomePage");
    }

    private static void DeleteLocalProfileData()
    {
        var profileFiles =
            Directory.GetFiles(
                FileSystem.AppDataDirectory,
                "profile_*");

        foreach (var filePath in profileFiles)
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // Continue deleting other locally stored files.
            }
        }

        Preferences.Default.Clear();
        SecureStorage.Default.RemoveAll();
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private void ClearError()
    {
        ErrorLabel.Text = string.Empty;
        ErrorLabel.IsVisible = false;
    }
}