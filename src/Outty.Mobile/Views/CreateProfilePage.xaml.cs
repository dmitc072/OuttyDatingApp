using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Outty.Mobile.Services;

namespace Outty.Mobile.Views;

public partial class CreateProfilePage : ContentPage
{
    private const int MaximumPhotos = 6;
    private const int MinimumAge = 18;

    private readonly ApiClient _apiClient;

    private bool _isSaving;

    public ObservableCollection<ProfilePhoto> Photos { get; } = new();

    public CreateProfilePage(ApiClient apiClient)
    {
        InitializeComponent();

        _apiClient = apiClient;

        BindingContext = this;

        ConfigureDatePicker();
        LoadSavedProfile();
    }

    private void ConfigureDatePicker()
    {
        DateTime today = DateTime.Today;

        DateOfBirthPicker.MaximumDate = today.AddYears(-MinimumAge);
        DateOfBirthPicker.MinimumDate = today.AddYears(-100);

        string savedDateOfBirth = Preferences.Default.Get(
            "ProfileDateOfBirth",
            string.Empty);

        if (DateTime.TryParse(savedDateOfBirth, out DateTime savedDate))
        {
            DateOfBirthPicker.Date = savedDate;
        }
        else
        {
            DateOfBirthPicker.Date = today.AddYears(-MinimumAge);
        }
    }

    private void LoadSavedProfile()
    {
        DisplayNameEntry.Text = Preferences.Default.Get(
            "ProfileDisplayName",
            string.Empty);

        BioEditor.Text = Preferences.Default.Get(
            "ProfileBio",
            string.Empty);

        CityEntry.Text = Preferences.Default.Get(
            "ProfileCity",
            string.Empty);

        ZipCodeEntry.Text = Preferences.Default.Get(
            "ProfileZipCode",
            string.Empty);

        string savedState = Preferences.Default.Get(
            "ProfileState",
            string.Empty);

        SelectPickerItem(StatePicker, savedState);

        int savedRadius = Preferences.Default.Get(
            "SearchRadiusMiles",
            25);

        savedRadius = Math.Clamp(savedRadius, 5, 100);

        SearchRadiusSlider.Value = savedRadius;
        SearchRadiusLabel.Text = $"{savedRadius} miles";

        LoadSavedInterests();
        LoadSavedPhotos();
    }

    private static void SelectPickerItem(
        Picker picker,
        string savedValue)
    {
        if (string.IsNullOrWhiteSpace(savedValue))
        {
            return;
        }

        for (int index = 0; index < picker.Items.Count; index++)
        {
            if (string.Equals(
                    picker.Items[index],
                    savedValue,
                    StringComparison.OrdinalIgnoreCase))
            {
                picker.SelectedIndex = index;
                return;
            }
        }
    }

    private void LoadSavedInterests()
    {
        string savedInterests = Preferences.Default.Get(
            "ProfileInterests",
            string.Empty);

        // Stored as "Name:Level" pairs, e.g. "Hiking:Advance,Camping:Beginner".
        Dictionary<string, string> savedLevelsByInterest = savedInterests
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries)
            .Select(entry => entry.Split(':', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => parts[0],
                parts => parts[1],
                StringComparer.OrdinalIgnoreCase);

        foreach ((CheckBox checkBox, Picker experiencePicker, string name) in GetInterestRows())
        {
            if (savedLevelsByInterest.TryGetValue(name, out string? level))
            {
                checkBox.IsChecked = true;
                experiencePicker.IsVisible = true;
                SelectPickerItem(experiencePicker, level);
            }
            else
            {
                checkBox.IsChecked = false;
                experiencePicker.IsVisible = false;
            }
        }
    }

    private void OnInterestCheckedChanged(
        object? sender,
        CheckedChangedEventArgs e)
    {
        CheckBox? checkBox = sender as CheckBox;

        foreach ((CheckBox rowCheckBox, Picker rowExperiencePicker, string _) in GetInterestRows())
        {
            if (ReferenceEquals(rowCheckBox, checkBox))
            {
                rowExperiencePicker.IsVisible = e.Value;

                if (!e.Value)
                {
                    rowExperiencePicker.SelectedIndex = -1;
                }

                break;
            }
        }
    }

    private (CheckBox CheckBox, Picker ExperiencePicker, string Name)[] GetInterestRows()
    {
        return
        [
            (HikingCheckBox, HikingExperiencePicker, "Hiking"),
            (CampingCheckBox, CampingExperiencePicker, "Camping"),
            (KayakingCheckBox, KayakingExperiencePicker, "Kayaking"),
            (FishingCheckBox, FishingExperiencePicker, "Fishing"),
            (CyclingCheckBox, CyclingExperiencePicker, "Cycling"),
            (RunningCheckBox, RunningExperiencePicker, "Running"),
            (ClimbingCheckBox, ClimbingExperiencePicker, "Climbing"),
            (OtherInterestCheckBox, OtherInterestExperiencePicker, "Other")
        ];
    }

    private List<(string Name, string Level)> GetSelectedInterestsWithLevels()
    {
        List<(string Name, string Level)> selections = new();

        foreach ((CheckBox checkBox, Picker experiencePicker, string name) in GetInterestRows())
        {
            if (checkBox.IsChecked)
            {
                selections.Add((name, experiencePicker.SelectedItem?.ToString() ?? string.Empty));
            }
        }

        return selections;
    }

    private bool AllSelectedInterestsHaveLevel()
    {
        foreach ((CheckBox checkBox, Picker experiencePicker, string _) in GetInterestRows())
        {
            if (checkBox.IsChecked && experiencePicker.SelectedIndex == -1)
            {
                return false;
            }
        }

        return true;
    }

    private void LoadSavedPhotos()
    {
        Photos.Clear();

        string savedPhotoJson = Preferences.Default.Get(
            "ProfilePhotos",
            string.Empty);

        if (string.IsNullOrWhiteSpace(savedPhotoJson))
        {
            LoadLegacyPrimaryPhoto();
            return;
        }

        try
        {
            List<string>? savedPaths =
                JsonSerializer.Deserialize<List<string>>(
                    savedPhotoJson);

            if (savedPaths is null)
            {
                return;
            }

            foreach (string path in savedPaths)
            {
                if (Photos.Count >= MaximumPhotos)
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(path) &&
                    File.Exists(path))
                {
                    Photos.Add(new ProfilePhoto
                    {
                        FilePath = path
                    });
                }
            }
        }
        catch (JsonException)
        {
            LoadLegacyPrimaryPhoto();
        }
    }

    private void LoadLegacyPrimaryPhoto()
    {
        string primaryPhoto = Preferences.Default.Get(
            "PrimaryProfilePhoto",
            string.Empty);

        if (!string.IsNullOrWhiteSpace(primaryPhoto) &&
            File.Exists(primaryPhoto))
        {
            Photos.Add(new ProfilePhoto
            {
                FilePath = primaryPhoto
            });
        }
    }

    private async void OnChoosePhotoClicked(
        object sender,
        EventArgs e)
    {
        if (!CanAddAnotherPhoto())
        {
            return;
        }

        try
        {
            FileResult? selectedPhoto =
                await MediaPicker.Default.PickPhotoAsync(
                    new MediaPickerOptions
                    {
                        Title = "Choose a profile photo"
                    });

            if (selectedPhoto is null)
            {
                return;
            }

            await AddPhotoAsync(selectedPhoto);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert(
                "Not Supported",
                "Photo selection is not supported on this device.",
                "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert(
                "Permission Required",
                "Outty needs permission to access your photos.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Photo Error",
                $"The photo could not be selected: {ex.Message}",
                "OK");
        }
    }

    private async void OnTakePhotoClicked(
        object sender,
        EventArgs e)
    {
        if (!CanAddAnotherPhoto())
        {
            return;
        }

        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert(
                    "Camera Unavailable",
                    "Photo capture is not supported on this device or emulator.",
                    "OK");

                return;
            }

            FileResult? capturedPhoto =
                await MediaPicker.Default.CapturePhotoAsync(
                    new MediaPickerOptions
                    {
                        Title = "Take a profile photo"
                    });

            if (capturedPhoto is null)
            {
                return;
            }

            await AddPhotoAsync(capturedPhoto);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert(
                "Not Supported",
                "The camera is not supported on this device.",
                "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert(
                "Permission Required",
                "Outty needs camera permission to take a photo.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Camera Error",
                $"The photo could not be captured: {ex.Message}",
                "OK");
        }
    }

    private bool CanAddAnotherPhoto()
    {
        if (Photos.Count < MaximumPhotos)
        {
            return true;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert(
                "Photo Limit Reached",
                $"You can add up to {MaximumPhotos} profile photos.",
                "OK");
        });

        return false;
    }

    private async Task AddPhotoAsync(FileResult photo)
    {
        if (Photos.Count >= MaximumPhotos)
        {
            return;
        }

        string extension = Path.GetExtension(photo.FileName);

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        string localFileName =
            $"profile_{Guid.NewGuid():N}{extension}";

        string localFilePath = Path.Combine(
            FileSystem.AppDataDirectory,
            localFileName);

        await using Stream sourceStream =
            await photo.OpenReadAsync();

        await using FileStream destinationStream =
            File.Create(localFilePath);

        await sourceStream.CopyToAsync(destinationStream);

        Photos.Add(new ProfilePhoto
        {
            FilePath = localFilePath
        });
    }

    private async void OnRemovePhotoClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ProfilePhoto photo)
        {
            return;
        }

        bool shouldRemove = await DisplayAlert(
            "Remove Photo",
            "Are you sure you want to remove this photo?",
            "Remove",
            "Cancel");

        if (!shouldRemove)
        {
            return;
        }

        Photos.Remove(photo);

        TryDeleteLocalPhoto(photo.FilePath);
    }

    private static void TryDeleteLocalPhoto(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath) ||
                !File.Exists(filePath))
            {
                return;
            }

            string appDataDirectory =
                Path.GetFullPath(FileSystem.AppDataDirectory);

            string photoPath = Path.GetFullPath(filePath);

            if (photoPath.StartsWith(
                    appDataDirectory,
                    StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(photoPath);
            }
        }
        catch
        {
            // Removing the photo from the profile should still succeed
            // even if the local file cannot be deleted.
        }
    }

    private void OnSearchRadiusChanged(
        object sender,
        ValueChangedEventArgs e)
    {
        int radius = (int)Math.Round(e.NewValue);

        radius = Math.Clamp(radius, 5, 100);

        SearchRadiusLabel.Text =
            radius == 1
                ? "1 mile"
                : $"{radius} miles";
    }

    private async void OnSaveProfileClicked(
        object sender,
        EventArgs e)
    {
        if (_isSaving)
        {
            return;
        }

        try
        {
            _isSaving = true;
            SetSavingState(true);

            string displayName =
                DisplayNameEntry.Text?.Trim() ?? string.Empty;

            string bio =
                BioEditor.Text?.Trim() ?? string.Empty;

            string city =
                CityEntry.Text?.Trim() ?? string.Empty;

            string state =
                StatePicker.SelectedItem?.ToString()?.Trim()
                ?? string.Empty;

            string zipCode =
                ZipCodeEntry.Text?.Trim() ?? string.Empty;

            DateTime? selectedDateOfBirth =
                DateOfBirthPicker.Date;

            if (!selectedDateOfBirth.HasValue)
            {
                await DisplayAlertAsync(
                    "Profile Incomplete",
                    "Please select your date of birth.",
                    "OK");

                return;
            }

            DateTime dateOfBirth =
                selectedDateOfBirth.Value;

            int searchRadius =
                (int)Math.Round(SearchRadiusSlider.Value);

            List<(string Name, string Level)> selectedInterests =
                GetSelectedInterestsWithLevels();

            string? validationMessage = ValidateProfile(
                displayName,
                bio,
                city,
                state,
                zipCode,
                dateOfBirth,
                selectedInterests);

            if (validationMessage is not null)
            {
                await DisplayAlert(
                    "Profile Incomplete",
                    validationMessage,
                    "OK");

                return;
            }

            if (!AllSelectedInterestsHaveLevel())
            {
                await DisplayAlert(
                    "Profile Incomplete",
                    "Please select an experience level for each checked interest.",
                    "OK");

                return;
            }

            int userId = Preferences.Default.Get("UserId", -1);

            if (userId == -1)
            {
                await DisplayAlert(
                    "Not Signed In",
                    "You must be signed in before creating a profile.",
                    "OK");

                return;
            }

            try
            {
                await _apiClient.CreateProfileAsync(
                    BuildCreateProfileRequest(
                        userId,
                        displayName,
                        bio,
                        city,
                        state,
                        zipCode,
                        dateOfBirth,
                        searchRadius,
                        selectedInterests));
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "Save Error",
                    $"Your profile could not be saved to the server: {ex.Message}",
                    "OK");

                return;
            }

            SaveProfileToPreferences(
                displayName,
                bio,
                city,
                state,
                zipCode,
                dateOfBirth,
                searchRadius,
                selectedInterests);

            await DisplayAlert(
                "Profile Saved",
                "Your Outty profile has been saved successfully.",
                "OK");

            await NavigateAfterSaveAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Save Error",
                $"Your profile could not be saved: {ex.Message}",
                "OK");
        }
        finally
        {
            _isSaving = false;
            SetSavingState(false);
        }
    }

    private static string? ValidateProfile(
        string displayName,
        string bio,
        string city,
        string state,
        string zipCode,
        DateTime dateOfBirth,
        IReadOnlyCollection<(string Name, string Level)> interests)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return "Please enter your display name.";
        }

        if (displayName.Length > 50)
        {
            return "Your display name must be 50 characters or fewer.";
        }

        if (!IsAtLeastAge(dateOfBirth, MinimumAge))
        {
            return $"You must be at least {MinimumAge} years old to create a profile.";
        }

        if (bio.Length > 500)
        {
            return "Your bio must be 500 characters or fewer.";
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return "Please enter your city.";
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return "Please select your state.";
        }

        if (!IsValidZipCode(zipCode))
        {
            return "Please enter a valid five-digit ZIP Code.";
        }

        if (interests.Count == 0)
        {
            return "Please select at least one outdoor interest.";
        }

        return null;
    }

    private static bool IsAtLeastAge(
        DateTime dateOfBirth,
        int minimumAge)
    {
        DateTime today = DateTime.Today;

        int age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age >= minimumAge;
    }

    private static bool IsValidZipCode(string zipCode)
    {
        return zipCode.Length == 5 &&
               zipCode.All(char.IsDigit);
    }

    private static CreateProfileRequest BuildCreateProfileRequest(
        int userId,
        string displayName,
        string bio,
        string city,
        string state,
        string zipCode,
        DateTime dateOfBirth,
        int searchRadius,
        IReadOnlyCollection<(string Name, string Level)> interests)
    {
        List<InterestSelection> interestSelections = interests
            .Select(interest =>
                new InterestSelection(interest.Name, interest.Level))
            .ToList();

        return new CreateProfileRequest(
            UserId: userId,
            DisplayName: displayName,
            BirthDate: DateOnly.FromDateTime(dateOfBirth),
            City: city,
            State: state,
            ZipCode: zipCode,
            Pronouns: null,
            Bio: bio,
            PreferredDistance: string.Empty,
            SearchRadiusMiles: searchRadius,
            Interests: interestSelections,
            Goals: []);
    }

    private void SaveProfileToPreferences(
        string displayName,
        string bio,
        string city,
        string state,
        string zipCode,
        DateTime dateOfBirth,
        int searchRadius,
        IReadOnlyCollection<(string Name, string Level)> interests)
    {
        Preferences.Default.Set(
            "ProfileDisplayName",
            displayName);

        Preferences.Default.Set(
            "ProfileBio",
            bio);

        Preferences.Default.Set(
            "ProfileDateOfBirth",
            dateOfBirth.ToString("O"));

        Preferences.Default.Set(
            "ProfileCity",
            city);

        Preferences.Default.Set(
            "ProfileState",
            state);

        Preferences.Default.Set(
            "ProfileZipCode",
            zipCode);

        Preferences.Default.Set(
            "ProfileInterests",
            string.Join(
                ",",
                interests.Select(interest => $"{interest.Name}:{interest.Level}")));

        Preferences.Default.Set(
            "SearchRadiusMiles",
            Math.Clamp(searchRadius, 5, 100));

        List<string> photoPaths = Photos
            .Where(photo =>
                !string.IsNullOrWhiteSpace(photo.FilePath))
            .Select(photo => photo.FilePath)
            .ToList();

        string photoJson =
            JsonSerializer.Serialize(photoPaths);

        Preferences.Default.Set(
            "ProfilePhotos",
            photoJson);

        Preferences.Default.Set(
            "PrimaryProfilePhoto",
            photoPaths.FirstOrDefault() ?? string.Empty);

        Preferences.Default.Set(
            "HasCompletedProfile",
            true);
    }

    private void SetSavingState(bool isSaving)
    {
        SaveProfileButton.IsEnabled = !isSaving;
        SaveProfileButton.Text =
            isSaving
                ? "Saving..."
                : "Save Profile";

        SavingIndicator.IsVisible = isSaving;
        SavingIndicator.IsRunning = isSaving;
    }

    private static async Task NavigateAfterSaveAsync()
    {
        try
        {
            await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
        }
        catch
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch
            {
                // The profile has still been saved even if navigation
                // cannot be completed.
            }
        }
    }
}

public sealed class ProfilePhoto
{
    public string FilePath { get; set; } = string.Empty;
}