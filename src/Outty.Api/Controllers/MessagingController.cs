using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;
using Outty.Shared.Models.Messaging;

namespace Outty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagingController : ControllerBase
{
    private readonly OuttyDbContext _dbContext;

    public MessagingController(OuttyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("users/{userId:int}/conversations")]
    public async Task<ActionResult<List<ConversationSummaryDto>>> GetConversations(
        int userId,
        CancellationToken cancellationToken)
    {
        var userExists = await _dbContext.Users
            .AnyAsync(user => user.Id == userId, cancellationToken);

        if (!userExists)
        {
            return NotFound($"User {userId} was not found.");
        }

        var conversationIds = await _dbContext.ConversationParticipants
            .Where(participant => participant.UserId == userId)
            .Select(participant => participant.ConversationId)
            .ToListAsync(cancellationToken);

        var conversations = await _dbContext.Conversations
            .Where(conversation => conversationIds.Contains(conversation.Id))
            .Select(conversation => new ConversationSummaryDto
            {
                ConversationId = conversation.Id,

                OtherUserId = conversation.ConversationParticipants
                    .Where(participant => participant.UserId != userId)
                    .Select(participant => participant.UserId)
                    .FirstOrDefault(),

                OtherUserName = conversation.ConversationParticipants
                    .Where(participant => participant.UserId != userId)
                    .Select(participant =>
                        participant.User.Profile != null
                            ? participant.User.Profile.DisplayName
                            : participant.User.Email)
                    .FirstOrDefault() ?? "Unknown User",

                LastMessage = conversation.Messages
                    .OrderByDescending(message => message.SentAtUtc)
                    .Select(message => message.Content)
                    .FirstOrDefault(),

                LastMessageAtUtc = conversation.Messages
                    .OrderByDescending(message => message.SentAtUtc)
                    .Select(message => (DateTime?)message.SentAtUtc)
                    .FirstOrDefault(),

                UnreadCount = conversation.Messages.Count(message =>
                    message.SenderId != userId &&
                    message.ReadAtUtc == null)
            })
            .OrderByDescending(conversation => conversation.LastMessageAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(conversations);
    }

    [HttpGet("conversations/{conversationId:int}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        int conversationId,
        [FromQuery] int currentUserId,
        CancellationToken cancellationToken)
    {
        var isParticipant = await _dbContext.ConversationParticipants
            .AnyAsync(
                participant =>
                    participant.ConversationId == conversationId &&
                    participant.UserId == currentUserId,
                cancellationToken);

        if (!isParticipant)
        {
            return Forbid();
        }

        var messages = await _dbContext.Messages
            .Where(message => message.ConversationId == conversationId)
            .OrderBy(message => message.SentAtUtc)
            .Select(message => new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender.Profile != null
                    ? message.Sender.Profile.DisplayName
                    : message.Sender.Email,
                Content = message.Content,
                SentAtUtc = message.SentAtUtc,
                ReadAtUtc = message.ReadAtUtc,
                IsCurrentUser = message.SenderId == currentUserId
            })
            .ToListAsync(cancellationToken);

        return Ok(messages);
    }

    [HttpPost("conversations")]
    public async Task<ActionResult<int>> CreateConversation(
        CreateConversationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserId == request.OtherUserId)
        {
            return BadRequest("A user cannot start a conversation with themselves.");
        }

        var usersExist = await _dbContext.Users
            .CountAsync(
                user =>
                    user.Id == request.CurrentUserId ||
                    user.Id == request.OtherUserId,
                cancellationToken);

        if (usersExist != 2)
        {
            return BadRequest("One or both users do not exist.");
        }

        var existingConversationId = await _dbContext.Conversations
            .Where(conversation =>
                conversation.ConversationParticipants.Count == 2 &&
                conversation.ConversationParticipants.Any(
                    participant => participant.UserId == request.CurrentUserId) &&
                conversation.ConversationParticipants.Any(
                    participant => participant.UserId == request.OtherUserId))
            .Select(conversation => (int?)conversation.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingConversationId.HasValue)
        {
            return Ok(existingConversationId.Value);
        }

        var conversation = new Conversation
        {
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            ConversationParticipants =
            [
                new ConversationParticipant
                {
                    UserId = request.CurrentUserId,
                    JoinedAtUtc = DateTime.UtcNow
                },
                new ConversationParticipant
                {
                    UserId = request.OtherUserId,
                    JoinedAtUtc = DateTime.UtcNow
                }
            ]
        };

        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(
            nameof(GetMessages),
            new
            {
                conversationId = conversation.Id,
                currentUserId = request.CurrentUserId
            },
            conversation.Id);
    }

    [HttpPost("conversations/{conversationId:int}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(
        int conversationId,
        SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Message content cannot be empty.");
        }

        var trimmedContent = request.Content.Trim();

        if (trimmedContent.Length > 2000)
        {
            return BadRequest("Message content cannot exceed 2,000 characters.");
        }

        var isParticipant = await _dbContext.ConversationParticipants
            .AnyAsync(
                participant =>
                    participant.ConversationId == conversationId &&
                    participant.UserId == request.SenderId,
                cancellationToken);

        if (!isParticipant)
        {
            return Forbid();
        }

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = request.SenderId,
            Content = trimmedContent,
            SentAtUtc = DateTime.UtcNow
        };

        _dbContext.Messages.Add(message);

        var conversation = await _dbContext.Conversations
            .FirstAsync(
                conversation => conversation.Id == conversationId,
                cancellationToken);

        conversation.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var senderName = await _dbContext.Users
            .Where(user => user.Id == request.SenderId)
            .Select(user =>
                user.Profile != null
                    ? user.Profile.DisplayName
                    : user.Email)
            .FirstAsync(cancellationToken);

        var response = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = senderName,
            Content = message.Content,
            SentAtUtc = message.SentAtUtc,
            ReadAtUtc = message.ReadAtUtc,
            IsCurrentUser = true
        };

        return Ok(response);
    }

    [HttpPut("conversations/{conversationId:int}/read")]
    public async Task<IActionResult> MarkMessagesRead(
        int conversationId,
        MarkMessagesReadRequest request,
        CancellationToken cancellationToken)
    {
        var isParticipant = await _dbContext.ConversationParticipants
            .AnyAsync(
                participant =>
                    participant.ConversationId == conversationId &&
                    participant.UserId == request.ReaderId,
                cancellationToken);

        if (!isParticipant)
        {
            return Forbid();
        }

        var unreadMessages = await _dbContext.Messages
            .Where(message =>
                message.ConversationId == conversationId &&
                message.SenderId != request.ReaderId &&
                message.ReadAtUtc == null)
            .ToListAsync(cancellationToken);

        if (unreadMessages.Count == 0)
        {
            return NoContent();
        }

        var readTime = DateTime.UtcNow;

        foreach (var message in unreadMessages)
        {
            message.ReadAtUtc = readTime;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}