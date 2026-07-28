using System.Net.Http.Json;
using Outty.Shared.Models.Messaging;

namespace Outty.Mobile.Services;

public class MessagingService : IMessagingService
{
    private readonly HttpClient _httpClient;

    public MessagingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ConversationSummaryDto>> GetConversationsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var conversations =
            await _httpClient.GetFromJsonAsync<List<ConversationSummaryDto>>(
                $"api/messaging/users/{userId}/conversations",
                cancellationToken);

        return conversations ?? [];
    }

    public async Task<List<MessageDto>> GetMessagesAsync(
        int conversationId,
        int currentUserId,
        CancellationToken cancellationToken = default)
    {
        var messages =
            await _httpClient.GetFromJsonAsync<List<MessageDto>>(
                $"api/messaging/conversations/{conversationId}/messages" +
                $"?currentUserId={currentUserId}",
                cancellationToken);

        return messages ?? [];
    }

    public async Task<int> CreateConversationAsync(
        int currentUserId,
        int otherUserId,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateConversationRequest
        {
            CurrentUserId = currentUserId,
            OtherUserId = otherUserId
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/messaging/conversations",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>(
            cancellationToken: cancellationToken);
    }

    public async Task<MessageDto> SendMessageAsync(
        int conversationId,
        int senderId,
        string content,
        CancellationToken cancellationToken = default)
    {
        var request = new SendMessageRequest
        {
            SenderId = senderId,
            Content = content
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"api/messaging/conversations/{conversationId}/messages",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var message = await response.Content.ReadFromJsonAsync<MessageDto>(
            cancellationToken: cancellationToken);

        return message
            ?? throw new InvalidOperationException(
                "The API returned an empty message response.");
    }

    public async Task MarkMessagesReadAsync(
        int conversationId,
        int readerId,
        CancellationToken cancellationToken = default)
    {
        var request = new MarkMessagesReadRequest
        {
            ReaderId = readerId
        };

        var response = await _httpClient.PutAsJsonAsync(
            $"api/messaging/conversations/{conversationId}/read",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}