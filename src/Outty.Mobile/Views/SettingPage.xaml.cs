namespace Outty.Mobile.Views;

public partial class SettingsPage : ContentPage
{
    private bool _isLoadingSettings;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadSettings();
    }

    private void LoadSettings()
    {
        _isLoadingSettings = true;

        try
        {
            NotificationsSwitch.IsToggled =
                Preferences.Default.Get(
                    "NotificationsEnabled",
                    true);

            LocationSwitch.IsToggled =
                Preferences.Default.Get(
                    "LocationSharingEnabled",
                    true);

            ShowProfileSwitch.IsToggled =
                Preferences.Default.Get(
                    "ShowProfileEnabled",
                    true);
        }
        finally
        {
            _isLoadingSettings = false;
        }
    }

    private void OnNotificationsToggled(
        object? sender,
        ToggledEventArgs e)
    {
        if (_isLoadingSettings)
        {
            return;
        }

        Preferences.Default.Set(
            "NotificationsEnabled",
            e.Value);
    }

    private void OnLocationToggled(
        object? sender,
        ToggledEventArgs e)
    {
        if (_isLoadingSettings)
        {
            return;
        }

        Preferences.Default.Set(
            "LocationSharingEnabled",
            e.Value);
    }

    private void OnShowProfileToggled(
        object? sender,
        ToggledEventArgs e)
    {
        if (_isLoadingSettings)
        {
            return;
        }

        Preferences.Default.Set(
            "ShowProfileEnabled",
            e.Value);
    }

    private async void OnProfileClicked(
        object? sender,
        EventArgs e)
    {
        await NavigateAsync(
            "//MainTabs/ProfilePage",
            "Unable to open your profile.");
    }

    private async void OnEditProfileClicked(
        object? sender,
        EventArgs e)
    {
        await NavigateAsync(
            nameof(CreateProfilePage),
            "Unable to open the profile editor.");
    }

    private async void OnDeleteAccountClicked(
        object? sender,
        EventArgs e)
    {
        await NavigateAsync(
            nameof(DeleteAccountPage),
            "Unable to open the delete account page.");
    }

    private async void OnPrivacyClicked(
        object? sender,
        EventArgs e)
    {
        await DisplayAlertAsync(
            "Privacy Information",
            "Outty stores profile information locally for this class version of the application.",
            "OK");
    }

    private async void OnHelpClicked(
        object? sender,
        EventArgs e)
    {
        await DisplayAlertAsync(
            "Help and Support",
            "For this class project, contact the Outty development team for assistance.",
            "OK");
    }

    private async void OnBackClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            await Shell.Current.GoToAsync(
                "//MainTabs/HomePage");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Navigation Error",
                $"Unable to return to the previous page: {ex.Message}",
                "OK");
        }
    }

    private static async Task NavigateAsync(
        string route,
        string errorMessage)
    {
        try
        {
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Navigation Error",
                $"{errorMessage} {ex.Message}",
                "OK");
        }
    }
}