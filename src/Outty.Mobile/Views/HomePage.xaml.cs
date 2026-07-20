namespace Outty.Mobile.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadProfile();
    }

    private void LoadProfile()
    {
        var name =
            Preferences.Default.Get(
                "ProfileName",
                "Adventurer");

        WelcomeNameLabel.Text = $"{name}!";
    }

    private async void OnStartExploringClicked(
        object? sender,
        EventArgs e)
    {
        await DisplayAlertAsync(
            "Coming Soon",
            "Adventure matching will be available in a future sprint.",
            "OK");
    }

    private async void OnProfileClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(
                nameof(ProfilePage));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Navigation Error",
                $"Unable to open your profile: {ex.Message}",
                "OK");
        }
    }

    private async void OnSettingsClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(
                nameof(SettingsPage));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Navigation Error",
                $"Unable to open settings: {ex.Message}",
                "OK");
        }
    }
}