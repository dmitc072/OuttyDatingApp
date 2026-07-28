using Outty.Mobile.Services;

namespace Outty.Mobile.Views;

public partial class MatchingPage : ContentPage
{
    private readonly ApiClient _apiClient;
    private readonly List<CandidateProfile> _candidates = [];
    private int _currentIndex;

    public MatchingPage(ApiClient apiClient)
    {
        InitializeComponent();
        _apiClient = apiClient;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = LoadCandidatesAsync();
    }

    private async Task LoadCandidatesAsync()
    {
        ShowLoading();

        var profileId = Preferences.Default.Get("ProfileId", -1);

        if (profileId < 0)
        {
            await DisplayAlert(
                "Profile Required",
                "Please finish creating your profile before you can start discovering people.",
                "OK");
            ShowEmpty();
            return;
        }

        try
        {
            var candidates = await _apiClient.GetCandidatesAsync(profileId);

            _candidates.Clear();
            _candidates.AddRange(candidates);
            _currentIndex = 0;

            ShowNextCandidate();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to load matches: {ex.Message}", "OK");
            ShowEmpty();
        }
    }

    private void ShowNextCandidate()
    {
        if (_currentIndex >= _candidates.Count)
        {
            ShowEmpty();
            return;
        }

        var candidate = _candidates[_currentIndex];

        CandidateNameLabel.Text = candidate.DisplayName;
        CandidateLocationLabel.Text = $"{candidate.City}, {candidate.State}";
        SharedInterestsLabel.Text = candidate.SharedInterestCount == 1
            ? "1 shared interest"
            : $"{candidate.SharedInterestCount} shared interests";

        var remaining = _candidates.Count - _currentIndex - 1;
        RemainingCountLabel.Text = remaining switch
        {
            0 => "Last one for now",
            1 => "1 more person after this",
            _ => $"{remaining} more people after this"
        };

        ShowCard();
    }

    private async void OnLikeClicked(object? sender, EventArgs e) => await SwipeAsync(liked: true);

    private async void OnPassClicked(object? sender, EventArgs e) => await SwipeAsync(liked: false);

    private async void OnRefreshClicked(object? sender, EventArgs e) => await LoadCandidatesAsync();

    private async void OnMatchesClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync(nameof(MatchesPage));

    private async void OnLikedClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync(nameof(LikedPage));

    private async Task SwipeAsync(bool liked)
    {
        if (_currentIndex >= _candidates.Count)
        {
            return;
        }

        var candidate = _candidates[_currentIndex];
        var myProfileId = Preferences.Default.Get("ProfileId", -1);

        SetButtonsEnabled(false);

        try
        {
            var result = await _apiClient.RecordSwipeAsync(myProfileId, candidate.ProfileId, liked);

            if (liked && result.IsMatch)
            {
                var shouldOpenChat = await DisplayAlert(
                    "It's a Match!",
                    $"You and {candidate.DisplayName} both liked each other.",
                    "Message Now",
                    "Keep Swiping");

                if (shouldOpenChat && result.ConversationId is int conversationId)
                {
                    await Shell.Current.GoToAsync(
                        $"{nameof(ChatPage)}?conversationId={conversationId}&otherUserName={Uri.EscapeDataString(candidate.DisplayName)}");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to record your swipe: {ex.Message}", "OK");
        }
        finally
        {
            SetButtonsEnabled(true);
        }

        _currentIndex++;
        ShowNextCandidate();
    }

    private void SetButtonsEnabled(bool enabled)
    {
        LikeButton.IsEnabled = enabled;
        PassButton.IsEnabled = enabled;
    }

    private void ShowLoading()
    {
        LoadingLayout.IsVisible = true;
        EmptyLayout.IsVisible = false;
        CardLayout.IsVisible = false;
    }

    private void ShowEmpty()
    {
        LoadingLayout.IsVisible = false;
        EmptyLayout.IsVisible = true;
        CardLayout.IsVisible = false;
    }

    private void ShowCard()
    {
        LoadingLayout.IsVisible = false;
        EmptyLayout.IsVisible = false;
        CardLayout.IsVisible = true;
    }
}
