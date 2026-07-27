using System.Collections.ObjectModel;
using System.Windows.Input;
using Outty.Mobile.Services;
using Outty.Shared.Models.Messaging;

namespace Outty.Mobile.Views;

public partial class ConversationsPage : ContentPage
{
    private readonly IMessagingService _messagingService;

    private bool _isLoading;
    private bool _isRefreshing;

    // Temporary user ID until authentication is fully connected.
    private const int CurrentUserId = 1;

    public ObservableCollection<ConversationSummaryDto> Conversations { get; }
        = [];

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

    public ConversationsPage(IMessagingService messagingService)
    {
        InitializeComponent();

        _messagingService = messagingService;

        RefreshCommand = new Command(
            async () => await RefreshConversationsAsync());

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
    base.OnAppearing();
    }

    private async Task LoadConversationsAsync()
    {
        if (_isLoading)
        {
            return;
        }

        try
        {
            _isLoading = true;

            var conversations =
                await _messagingService.GetConversationsAsync(CurrentUserId);

            Conversations.Clear();

            foreach (var conversation in conversations)
            {
                Conversations.Add(conversation);
            }
        }
        catch (HttpRequestException exception)
        {
            await DisplayAlertAsync(
                "Unable to load messages",
                $"The messaging service could not be reached. {exception.Message}",
                "OK");
        }
        catch (Exception exception)
        {
            await DisplayAlertAsync(
                "Error",
                $"Messages could not be loaded. {exception.Message}",
                "OK");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task RefreshConversationsAsync()
    {
        try
        {
            IsRefreshing = true;
            await LoadConversationsAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnConversationSelected(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault()
            is not ConversationSummaryDto conversation)
        {
            return;
        }

        ConversationsCollectionView.SelectedItem = null;

        var route =
            $"{nameof(ChatPage)}" +
            $"?conversationId={conversation.ConversationId}" +
            $"&otherUserName={Uri.EscapeDataString(conversation.OtherUserName)}";

        await Shell.Current.GoToAsync(route);
    }
}