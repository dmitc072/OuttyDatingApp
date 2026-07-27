using Microsoft.Maui.Storage;

namespace Outty.Mobile.Views;

public partial class DeleteAccountPage : ContentPage
{
    private bool _isDeleting;

    public DeleteAccountPage()
    {
        InitializeComponent();
    }

    private void OnConfirmationTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        DeleteAccountButton.IsEnabled =
            string.Equals(
                e.NewTextValue?.Trim(),
                "DELETE",
                StringComparison.OrdinalIgnoreCase);
    }

    private async void OnDeleteAccountClicked(
        object? sender,
        EventArgs e)
    {
        if (_isDeleting)
        {
            return;
        }

        string confirmation =
            ConfirmationEntry.Text?.Trim() ?? string.Empty;

        if (!string.Equals(
                confirmation,
                "DELETE",
                StringComparison.OrdinalIgnoreCase))
        {
            await DisplayAlertAsync(
                "Confirmation Required",
                "Please type DELETE to confirm.",
                "OK");

            return;
        }

        bool confirmed = await DisplayAlertAsync(
            "Delete Account",
            "Are you sure you want to permanently delete your account?",
            "Delete",
            "Cancel");

        if (!confirmed)
        {
            return;
        }

        try
        {
            _isDeleting = true;

            DeleteAccountButton.IsEnabled = false;
            DeletingIndicator.IsVisible = true;
            DeletingIndicator.IsRunning = true;

            DeleteSavedProfile();

            await DisplayAlertAsync(
                "Account Deleted",
                "Your locally saved Outty profile has been deleted.",
                "OK");

            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Delete Error",
                $"Your account could not be deleted: {ex.Message}",
                "OK");
        }
        finally
        {
            _isDeleting = false;
            DeletingIndicator.IsVisible = false;
            DeletingIndicator.IsRunning = false;
        }
    }

    private static void DeleteSavedProfile()
    {
        string[] profilePreferenceKeys =
        {
            "ProfileDisplayName",
            "ProfileBio",
            "ProfileDateOfBirth",
            "ProfileCity",
            "ProfileState",
            "ProfileZipCode",
            "ProfileExperienceLevel",
            "ProfileInterests",
            "ProfilePhotos",
            "PrimaryProfilePhoto",
            "SearchRadiusMiles",
            "HasCompletedProfile"
        };

        foreach (string key in profilePreferenceKeys)
        {
            Preferences.Default.Remove(key);
        }
    }

    private async void OnCancelClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}