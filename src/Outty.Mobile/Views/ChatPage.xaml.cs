using System.Collections.ObjectModel;
using System.Windows.Input;
using Outty.Mobile.Services;
using Outty.Shared.Models.Messaging;

namespace Outty.Mobile.Views;

public partial class ChatPage : ContentPage, IQueryAttributable
{
    private readonly IMessagingService _messagingService;

    // Temporary user ID until authentication is fully connected.
    private const int CurrentUserId = 1;

    private int _conversationId;
    private string _otherUserName = "Chat";
    private string _newMessageText = string.Empty;
    private bool _isLoading;
    private bool _isRefreshing;
    private bool _hasLoaded;

    public ObservableCollection<MessageDto> Messages { get; } = [];

    public int ConversationId
    {
        get => _conversationId;
        private set
        {
            if (_conversationId == value)
            {
                return;
            }

            _conversationId = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSendMessage));

            ((Command)SendMessageCommand).ChangeCanExecute();
        }
    }

    public string OtherUserName
    {
        get => _otherUserName;
        private set
        {
            if (_otherUserName == value)
            {
                return;
            }

            _otherUserName = value;
            OnPropertyChanged();
        }
    }

    public string NewMessageText
    {
        get => _newMessageText;
        set
        {
            if (_newMessageText == value)
            {
                return;
            }

            _newMessageText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSendMessage));

            ((Command)SendMessageCommand).ChangeCanExecute();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading == value)
            {
                return;
            }

            _isLoading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSendMessage));

            ((Command)SendMessageCommand).ChangeCanExecute();
        }
    }

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

    public bool CanSendMessage =>
        !IsLoading &&
        ConversationId > 0 &&
        !string.IsNullOrWhiteSpace(NewMessageText);

    public ICommand SendMessageCommand { get; }

    public ICommand RefreshCommand { get; }

    public ChatPage(IMessagingService messagingService)
    {
        InitializeComponent();

        _messagingService = messagingService;

        SendMessageCommand = new Command(
            async () => await SendMessageAsync(),
            () => CanSendMessage);

        RefreshCommand = new Command(
            async () => await RefreshMessagesAsync());

        BindingContext = this;
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue(
                "conversationId",
                out var conversationValue) &&
            int.TryParse(
                conversationValue?.ToString(),
                out var conversationId))
        {
            ConversationId = conversationId;
        }

        if (query.TryGetValue(
                "otherUserName",
                out var nameValue))
        {
            OtherUserName = Uri.UnescapeDataString(
                nameValue?.ToString() ?? "Chat");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasLoaded || ConversationId <= 0)
        {
            return;
        }

        _hasLoaded = true;

        await LoadMessagesAsync();
    }

    private async Task LoadMessagesAsync()
    {
        if (IsLoading || ConversationId <= 0)
        {
            return;
        }

        try
        {
            IsLoading = true;

            var messages =
                await _messagingService.GetMessagesAsync(
                    ConversationId,
                    CurrentUserId,
                    CancellationToken.None);

            Messages.Clear();

            foreach (var message in messages.OrderBy(
                         message => message.SentAtUtc))
            {
                Messages.Add(message);
            }

            ScrollToLatestMessage();

            await MarkMessagesReadAsync();
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
                $"The conversation could not be loaded. {exception.Message}",
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshMessagesAsync()
    {
        try
        {
            IsRefreshing = true;

            await LoadMessagesAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task SendMessageAsync()
    {
        var messageText = NewMessageText.Trim();

        if (string.IsNullOrWhiteSpace(messageText))
        {
            return;
        }

        try
        {
            IsLoading = true;

            var sentMessage =
                await _messagingService.SendMessageAsync(
                    ConversationId,
                    CurrentUserId,
                    messageText,
                    CancellationToken.None);

            Messages.Add(sentMessage);

            NewMessageText = string.Empty;

            ScrollToLatestMessage();
        }
        catch (HttpRequestException exception)
        {
            await DisplayAlertAsync(
                "Unable to send message",
                $"The messaging service could not be reached. {exception.Message}",
                "OK");
        }
        catch (Exception exception)
        {
            await DisplayAlertAsync(
                "Message not sent",
                exception.Message,
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task MarkMessagesReadAsync()
    {
        try
        {
            await _messagingService.MarkMessagesReadAsync(
                ConversationId,
                CurrentUserId,
                CancellationToken.None);
        }
        catch
        {
            // Do not prevent the conversation from loading
            // if marking messages as read fails.
        }
    }

    private void ScrollToLatestMessage()
    {
        if (Messages.Count == 0)
        {
            return;
        }

        Dispatcher.Dispatch(() =>
        {
            MessagesCollectionView.ScrollTo(
                Messages[^1],
                position: ScrollToPosition.End,
                animate: true);
        });
    }
}