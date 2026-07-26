using System.Collections.ObjectModel;
using Outty.Mobile.Models;
using Outty.Shared.Utilities;

namespace Outty.Mobile.Views;

public partial class CreateProfilePage : ContentPage
{
    private const int MaximumPhotos = 6;

    public ObservableCollection<ProfilePhoto> Photos { get; } = [];

    public CreateProfilePage()
    {
        InitializeComponent();

        BindingContext = this;

        BirthDatePicker.MaximumDate = DateTime.Today;
        BirthDatePicker.MinimumDate = DateTime.Today.AddYears(-100);
        BirthDatePicker.Date = DateTime.Today.AddYears(-18);

        UpdateAgeLabel();
        UpdatePhotoCount();
    }

    private async void OnChoosePhotosClicked(
        object? sender,
        EventArgs e)
    {
        ClearError();

        if (Photos.Count >= MaximumPhotos)
        {
            ShowError(
                $"You may upload a maximum of {MaximumPhotos} photos.");

            return;
        }

        try
        {
            IEnumerable<FileResult> selectedPhotos;

            try
            {
                selectedPhotos = await MediaPicker.Default.PickPhotosAsync(
                    new MediaPickerOptions
                    {
                        Title = "Select profile photos"
                    });
            }
            catch (Exception ex) when (ex.Message.Contains("No Activity found to handle Intent"))
            {
                // Falls back to the Storage Access Framework file picker when the
                // system Photo Picker (Android 13+, or 11-12 with the Play module)
                // isn't available on this device/emulator image.
                var pickedFiles = await FilePicker.Default.PickMultipleAsync(
                    new PickOptions
                    {
                        PickerTitle = "Select profile photos",
                        FileTypes = FilePickerFileType.Images
                    });

                selectedPhotos = pickedFiles?.OfType<FileResult>() ?? [];
            }

            if (selectedPhotos is null)
            {
                return;
            }

            foreach (var selectedPhoto in selectedPhotos)
            {
                if (Photos.Count >= MaximumPhotos)
                {
                    break;
                }

                await AddPhotoAsync(selectedPhoto);
            }

            UpdatePhotoCount();
        }
        catch (PermissionException)
        {
            ShowError(
                "Outty does not have permission to access your photos.");
        }
        catch (Exception ex)
        {
            ShowError(
                $"Unable to select photos: {ex.Message}");
        }
    }

    private async void OnTakePhotoClicked(
        object? sender,
        EventArgs e)
    {
        ClearError();

        if (Photos.Count >= MaximumPhotos)
        {
            ShowError(
                $"You may upload a maximum of {MaximumPhotos} photos.");

            return;
        }

        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlertAsync(
                    "Camera Unavailable",
                    "Photo capture is not supported on this device.",
                    "OK");

                return;
            }

            var cameraPermission =
                await Permissions.RequestAsync<Permissions.Camera>();

            if (cameraPermission != PermissionStatus.Granted)
            {
                ShowError(
                    "Camera permission is required to take a photo.");

                return;
            }

            var capturedPhoto =
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
            UpdatePhotoCount();
        }
        catch (PermissionException)
        {
            ShowError(
                "Outty does not have permission to use the camera.");
        }
        catch (Exception ex)
        {
            ShowError(
                $"Unable to take photo: {ex.Message}");
        }
    }

    private async Task AddPhotoAsync(FileResult selectedPhoto)
    {
        var fileExtension =
            Path.GetExtension(selectedPhoto.FileName);

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            fileExtension = ".jpg";
        }

        var localFileName =
            $"profile_{Guid.NewGuid()}{fileExtension}";

        var localFilePath =
            Path.Combine(
                FileSystem.AppDataDirectory,
                localFileName);

        await using var sourceStream =
            await selectedPhoto.OpenReadAsync();

        await using var localFileStream =
            File.Create(localFilePath);

        await sourceStream.CopyToAsync(localFileStream);

        Photos.Add(
            new ProfilePhoto
            {
                FileName = selectedPhoto.FileName,
                FilePath = localFilePath
            });
    }

    private void OnRemovePhotoClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ProfilePhoto photo)
        {
            return;
        }

        Photos.Remove(photo);

        try
        {
            if (File.Exists(photo.FilePath))
            {
                File.Delete(photo.FilePath);
            }
        }
        catch
        {
            // Keep the app running if the local file cannot be deleted.
        }

        UpdatePhotoCount();
    }

    private void OnBioTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        BioCountLabel.Text =
            $"{e.NewTextValue?.Length ?? 0} / 300";
    }

    private void OnBirthDateSelected(
        object? sender,
        DateChangedEventArgs e)
    {
        UpdateAgeLabel();
    }

    private void UpdateAgeLabel()
    {
        var birthDate =
            BirthDatePicker.Date ?? DateTime.Today;

        var age = AgeCalculator.CalculateAge(birthDate, DateTime.Today);
        var isEligible = AgeCalculator.IsAtLeast18(birthDate, DateTime.Today);

        AgeLabel.Text = isEligible
            ? $"Age: {age}"
            : "You must be at least 18 years old.";

        AgeLabel.TextColor = isEligible
            ? Color.FromArgb("#667267")
            : Color.FromArgb("#B3261E");
    }

    private async void OnSaveProfileClicked(
        object? sender,
        EventArgs e)
    {
        ClearError();

        if (Photos.Count == 0)
        {
            ShowError(
                "Please add at least one profile photo.");

            return;
        }

        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            ShowError(
                "Please enter a display name.");

            return;
        }

        var birthDate =
            BirthDatePicker.Date ?? DateTime.Today;

        if (!AgeCalculator.IsAtLeast18(birthDate, DateTime.Today))
        {
            ShowError(
                "You must be at least 18 years old.");

            return;
        }

        if (string.IsNullOrWhiteSpace(LocationEntry.Text))
        {
            ShowError(
                "Please enter your location.");

            return;
        }

        if (string.IsNullOrWhiteSpace(BioEditor.Text))
        {
            ShowError(
                "Please enter a short bio.");

            return;
        }

        if (DistancePicker.SelectedIndex == -1)
        {
            ShowError(
                "Please select a preferred adventure distance.");

            return;
        }

        if (!HasSelectedGoal())
        {
            ShowError(
                "Please select what you are looking for.");

            return;
        }

        if (!HasSelectedInterest())
        {
            ShowError(
                "Please select at least one outdoor interest.");

            return;
        }

        if (!SelectedInterestsHaveExperienceLevel())
        {
            ShowError(
                "Please select an experience level for each checked interest.");

            return;
        }

        SaveProfileInformation();

        await DisplayAlertAsync(
            "Profile Created",
            $"Your profile has been created with {Photos.Count} photo(s).",
            "Continue");

        await Shell.Current.GoToAsync("//HomePage");
    }

    private void SaveProfileInformation()
    {
        Preferences.Default.Set(
            "ProfileName",
            NameEntry.Text?.Trim() ?? string.Empty);

        Preferences.Default.Set(
            "ProfileLocation",
            LocationEntry.Text?.Trim() ?? string.Empty);

        Preferences.Default.Set(
            "ProfilePronouns",
            PronounsPicker.SelectedItem?.ToString()
            ?? "Not provided");

        Preferences.Default.Set(
            "ProfileBio",
            BioEditor.Text?.Trim() ?? string.Empty);

        Preferences.Default.Set(
            "ProfileDistance",
            DistancePicker.SelectedItem?.ToString()
            ?? "Not selected");

        Preferences.Default.Set(
            "ProfileLookingFor",
            BuildLookingForText());

        Preferences.Default.Set(
            "ProfileInterests",
            BuildInterestsText());

        Preferences.Default.Set(
            "ProfileBirthDate",
            (BirthDatePicker.Date ?? DateTime.Today)
                .ToString("O"));

        Preferences.Default.Set(
            "PrimaryProfilePhoto",
            Photos[0].FilePath);
    }

    private string BuildLookingForText()
    {
        var selections = new List<string>();

        if (FriendsCheckBox.IsChecked)
        {
            selections.Add("Friends");
        }

        if (PartnersCheckBox.IsChecked)
        {
            selections.Add("Adventure Partners");
        }

        if (DatingCheckBox.IsChecked)
        {
            selections.Add("Dating");
        }

        if (GroupsCheckBox.IsChecked)
        {
            selections.Add("Group Activities");
        }

        return string.Join(", ", selections);
    }

    private void OnInterestCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;

        foreach (var row in GetInterestRows())
        {
            if (row.CheckBox == checkBox)
            {
                row.ExperiencePicker.IsVisible = e.Value;

                if (!e.Value)
                {
                    row.ExperiencePicker.SelectedIndex = -1;
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
            (ClimbingCheckBox, ClimbingExperiencePicker, "Rock Climbing"),
            (CyclingCheckBox, CyclingExperiencePicker, "Cycling"),
            (TravelCheckBox, TravelExperiencePicker, "Travel and Road Trips")
        ];
    }

    private string BuildInterestsText()
    {
        var selections = new List<string>();

        foreach (var row in GetInterestRows())
        {
            if (row.CheckBox.IsChecked)
            {
                selections.Add(
                    $"{row.Name} ({row.ExperiencePicker.SelectedItem})");
            }
        }

        return string.Join(", ", selections);
    }

    private bool SelectedInterestsHaveExperienceLevel()
    {
        foreach (var row in GetInterestRows())
        {
            if (row.CheckBox.IsChecked && row.ExperiencePicker.SelectedIndex == -1)
            {
                return false;
            }
        }

        return true;
    }

    private bool HasSelectedGoal()
    {
        return FriendsCheckBox.IsChecked ||
               PartnersCheckBox.IsChecked ||
               DatingCheckBox.IsChecked ||
               GroupsCheckBox.IsChecked;
    }

    private bool HasSelectedInterest()
    {
        return HikingCheckBox.IsChecked ||
               CampingCheckBox.IsChecked ||
               KayakingCheckBox.IsChecked ||
               ClimbingCheckBox.IsChecked ||
               CyclingCheckBox.IsChecked ||
               TravelCheckBox.IsChecked;
    }

    private void UpdatePhotoCount()
    {
        PhotoCountLabel.Text =
            $"{Photos.Count} / {MaximumPhotos}";
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