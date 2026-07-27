using System.Collections.ObjectModel;
using System.Windows.Input;
using Outty.Mobile.Services;

namespace Outty.Mobile.Views;

public class LikedProfileDisplay
{
    public required int ProfileId { get; init; }
    public required string DisplayName { get; init; }
    public required string Location { get; init; }
    public required bool IsMatch { get; init; }
    public required int? ConversationId { get; init; }

    public string StatusText => IsMatch ? "💬 Matched" : "⏳ Waiting";
    public Color StatusBackgroundColor => IsMatch ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#EEEEEE");
    public Color StatusTextColor => IsMatch ? Color.FromArgb("#2E7D32") : Color.FromArgb("#757575");
}

public partial class LikedPage : ContentPage
{
    private readonly ApiClient _apiClient;

    private bool _isLoading;
    private bool _isRefreshing;

    public ObservableCollection<LikedProfileDisplay> LikedProfiles { get; } = [];

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            if (_isRefreshing == value)
            {
                return;
            }

            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }

    public LikedPage(ApiClient apiClient)
    {
        InitializeComponent();

        _apiClient = apiClient;

        RefreshCommand = new Command(async () => await RefreshLikedProfilesAsync());

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = LoadLikedProfilesAsync();
    }

    private async Task LoadLikedProfilesAsync()
    {
        if (_isLoading)
        {
            return;
        }

        var profileId = Preferences.Default.Get("ProfileId", -1);

        if (profileId < 0)
        {
            ShowEmpty();
            return;
        }

        try
        {
            _isLoading = true;

            var likedProfiles = await _apiClient.GetLikedProfilesAsync(profileId);

            LikedProfiles.Clear();

            foreach (var liked in likedProfiles)
            {
                LikedProfiles.Add(new LikedProfileDisplay
                {
                    ProfileId = liked.ProfileId,
                    DisplayName = liked.DisplayName,
                    Location = $"{liked.City}, {liked.State}",
                    IsMatch = liked.IsMatch,
                    ConversationId = liked.ConversationId
                });
            }

            if (LikedProfiles.Count == 0)
            {
                ShowEmpty();
            }
            else
            {
                ShowLiked();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to load your liked profiles: {ex.Message}", "OK");
            ShowEmpty();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task RefreshLikedProfilesAsync()
    {
        try
        {
            IsRefreshing = true;
            await LoadLikedProfilesAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnLikedProfileTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not LikedProfileDisplay liked)
        {
            return;
        }

        if (!liked.IsMatch || liked.ConversationId is not int conversationId)
        {
            await DisplayAlert(
                "Not Matched Yet",
                $"You'll be able to chat with {liked.DisplayName} once they like you back too.",
                "OK");
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ChatPage)}?conversationId={conversationId}&otherUserName={Uri.EscapeDataString(liked.DisplayName)}");
    }

    private void ShowEmpty()
    {
        LoadingLayout.IsVisible = false;
        EmptyLayout.IsVisible = true;
        LikedRefreshView.IsVisible = false;
    }

    private void ShowLiked()
    {
        LoadingLayout.IsVisible = false;
        EmptyLayout.IsVisible = false;
        LikedRefreshView.IsVisible = true;
    }
}
