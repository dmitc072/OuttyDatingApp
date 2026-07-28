using Outty.Shared.Models.Messaging;

namespace Outty.Mobile.Services;

public interface IMessagingService
{
    Task<List<ConversationSummaryDto>> GetConversationsAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<List<MessageDto>> GetMessagesAsync(
        int conversationId,
        int currentUserId,
        CancellationToken cancellationToken = default);

    Task<int> CreateConversationAsync(
        int currentUserId,
        int otherUserId,
        CancellationToken cancellationToken = default);

    Task<MessageDto> SendMessageAsync(
        int conversationId,
        int senderId,
        string content,
        CancellationToken cancellationToken = default);

    Task MarkMessagesReadAsync(
        int conversationId,
        int readerId,
        CancellationToken cancellationToken = default);
}