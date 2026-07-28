using System.Collections.ObjectModel;
using System.Windows.Input;
using Outty.Mobile.Services;

namespace Outty.Mobile.Views;

public partial class MatchesPage : ContentPage
{
    private readonly ApiClient _apiClient;

    private bool _isLoading;
    private bool _isRefreshing;

    public ObservableCollection<MatchSummary> Matches { get; } = [];

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

    public MatchesPage(ApiClient apiClient)
    {
        InitializeComponent();

        _apiClient = apiClient;

        RefreshCommand = new Command(async () => await RefreshMatchesAsync());

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = LoadMatchesAsync();
    }

    private async Task LoadMatchesAsync()
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

            var matches = await _apiClient.GetMatchesAsync(profileId);

            Matches.Clear();

            foreach (var match in matches)
            {
                Matches.Add(match);
            }

            if (Matches.Count == 0)
            {
                ShowEmpty();
            }
            else
            {
                ShowMatches();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to load your matches: {ex.Message}", "OK");
            ShowEmpty();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task RefreshMatchesAsync()
    {
        try
        {
            IsRefreshing = true;
            await LoadMatchesAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnMatchTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not MatchSummary match)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ChatPage)}?conversationId={match.ConversationId}&otherUserName={Uri.EscapeDataString(match.DisplayName)}");
    }

    private void ShowEmpty()
    {
        LoadingLayout.IsVisible = false;
        EmptyLayout.IsVisible = true;
        MatchesRefreshView.IsVisible = false;
    }

    private void ShowMatches()
    {
        LoadingLayout.IsVisible = false;
        EmptyLayout.IsVisible = false;
        MatchesRefreshView.IsVisible = true;
    }
}
