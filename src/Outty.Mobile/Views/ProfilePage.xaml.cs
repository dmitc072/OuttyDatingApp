namespace Outty.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadProfileInformation();
    }

    private void LoadProfileInformation()
    {
        ProfileNameLabel.Text =
            Preferences.Default.Get(
                "ProfileName",
                "Outty User");

        ProfileLocationLabel.Text =
            BuildLocationText();

        ProfilePronounsLabel.Text =
            Preferences.Default.Get(
                "ProfilePronouns",
                "Pronouns not added");

        ProfileBioLabel.Text =
            Preferences.Default.Get(
                "ProfileBio",
                "No bio has been added yet.");

        DistanceLabel.Text =
            Preferences.Default.Get(
                "ProfileDistance",
                "Not selected");

        LookingForLabel.Text =
            Preferences.Default.Get(
                "ProfileLookingFor",
                "Not selected");

        InterestsLabel.Text =
            BuildInterestsText();

        var primaryPhotoPath =
            Preferences.Default.Get(
                "PrimaryProfilePhoto",
                string.Empty);

        if (!string.IsNullOrWhiteSpace(primaryPhotoPath) &&
            File.Exists(primaryPhotoPath))
        {
            ProfileImage.Source =
                ImageSource.FromFile(primaryPhotoPath);
        }
        else
        {
            ProfileImage.Source = null;

            ProfileImage.BackgroundColor =
                Color.FromArgb("#E4EAE3");
        }
    }

    private string BuildLocationText()
    {
        var city = Preferences.Default.Get("ProfileCity", string.Empty);
        var state = Preferences.Default.Get("ProfileState", string.Empty);

        if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state))
        {
            return "Location not added";
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return city;
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return state;
        }

        return $"{city}, {state}";
    }

    private string BuildInterestsText()
    {
        var saved = Preferences.Default.Get("ProfileInterests", string.Empty);

        if (string.IsNullOrWhiteSpace(saved))
        {
            return "No interests selected.";
        }

        // Stored as "Name:Level" pairs, e.g. "Hiking:Advance,Camping:Beginner".
        var formatted = saved
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(entry =>
            {
                var parts = entry.Split(':', 2);
                return parts.Length == 2
                    ? $"{parts[0]} ({parts[1]})"
                    : parts[0];
            });

        return string.Join(", ", formatted);
    }

    private async void OnEditProfileClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(
                nameof(CreateProfilePage));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Navigation Error",
                $"Unable to open the profile editor: {ex.Message}",
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

    private async void OnHomeClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(
                $"///{nameof(HomePage)}");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Navigation Error",
                $"Unable to return home: {ex.Message}",
                "OK");
        }
    }
}