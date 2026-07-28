using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outty.Api.Controllers;
using Outty.Api.Data;
using Outty.Shared.Models.Messaging;

namespace Outty.Api.Tests;

public class MessagingControllerTests
{
    private static OuttyDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<OuttyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static User AddUser(OuttyDbContext db, string email)
    {
        var user = new User { Email = email, CreatedAtUtc = DateTime.UtcNow };
        db.Users.Add(user);
        return user;
    }

    // CreateConversation returns via CreatedAtAction (new conversation) or Ok
    // (existing conversation) — both are IActionResult, so the value lives on
    // ActionResult<T>.Result, not .Value (which stays default for those paths).
    private static int ExtractConversationId(ActionResult<int> result)
    {
        return result.Result switch
        {
            CreatedAtActionResult created => Assert.IsType<int>(created.Value),
            OkObjectResult ok => Assert.IsType<int>(ok.Value),
            _ => throw new InvalidOperationException(
                $"Unexpected result type: {result.Result?.GetType()}")
        };
    }

    [Fact]
    public async Task CreateConversation_BetweenTwoExistingUsers_CreatesAConversation()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        var bob = AddUser(db, "bob@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var result = await controller.CreateConversation(
            new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = bob.Id },
            CancellationToken.None);

        var conversationId = ExtractConversationId(result);

        var conversation = await db.Conversations.FindAsync(conversationId);
        Assert.NotNull(conversation);

        var participantIds = await db.ConversationParticipants
            .Where(p => p.ConversationId == conversationId)
            .Select(p => p.UserId)
            .ToListAsync();

        Assert.Contains(alice.Id, participantIds);
        Assert.Contains(bob.Id, participantIds);
    }

    [Fact]
    public async Task CreateConversation_WhenOneAlreadyExistsBetweenTheSameTwoUsers_ReturnsTheExistingOne()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        var bob = AddUser(db, "bob@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var request = new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = bob.Id };

        var first = await controller.CreateConversation(request, CancellationToken.None);
        var firstId = ExtractConversationId(first);

        var second = await controller.CreateConversation(request, CancellationToken.None);
        var secondId = ExtractConversationId(second);

        Assert.Equal(firstId, secondId);
        Assert.Equal(1, await db.Conversations.CountAsync());
    }

    [Fact]
    public async Task CreateConversation_WhenBothUserIdsAreTheSame_ReturnsBadRequest()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var result = await controller.CreateConversation(
            new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = alice.Id },
            CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SendMessage_FromAParticipant_AddsTheMessageAndUpdatesConversationTimestamp()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        var bob = AddUser(db, "bob@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var createResult = await controller.CreateConversation(
            new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = bob.Id },
            CancellationToken.None);
        var conversationId = ExtractConversationId(createResult);

        var sendResult = await controller.SendMessage(
            conversationId,
            new SendMessageRequest { SenderId = alice.Id, Content = "Hey, want to go hiking?" },
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(sendResult.Result);
        var message = Assert.IsType<MessageDto>(okResult.Value);
        Assert.Equal("Hey, want to go hiking?", message.Content);
        Assert.Equal(alice.Id, message.SenderId);

        Assert.Equal(1, await db.Messages.CountAsync());
    }

    [Fact]
    public async Task SendMessage_FromANonParticipant_ReturnsForbidden()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        var bob = AddUser(db, "bob@example.com");
        var eve = AddUser(db, "eve@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var createResult = await controller.CreateConversation(
            new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = bob.Id },
            CancellationToken.None);
        var conversationId = ExtractConversationId(createResult);

        var sendResult = await controller.SendMessage(
            conversationId,
            new SendMessageRequest { SenderId = eve.Id, Content = "I'm not in this conversation." },
            CancellationToken.None);

        Assert.IsType<ForbidResult>(sendResult.Result);
    }

    [Fact]
    public async Task SendMessage_WithEmptyContent_ReturnsBadRequest()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        var bob = AddUser(db, "bob@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var createResult = await controller.CreateConversation(
            new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = bob.Id },
            CancellationToken.None);
        var conversationId = ExtractConversationId(createResult);

        var sendResult = await controller.SendMessage(
            conversationId,
            new SendMessageRequest { SenderId = alice.Id, Content = "   " },
            CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(sendResult.Result);
    }

    [Fact]
    public async Task MarkMessagesRead_MarksOnlyTheReaderSUnreadMessages()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        var bob = AddUser(db, "bob@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var createResult = await controller.CreateConversation(
            new CreateConversationRequest { CurrentUserId = alice.Id, OtherUserId = bob.Id },
            CancellationToken.None);
        var conversationId = ExtractConversationId(createResult);

        await controller.SendMessage(
            conversationId,
            new SendMessageRequest { SenderId = alice.Id, Content = "First message" },
            CancellationToken.None);
        await controller.SendMessage(
            conversationId,
            new SendMessageRequest { SenderId = alice.Id, Content = "Second message" },
            CancellationToken.None);

        await controller.MarkMessagesRead(
            conversationId,
            new MarkMessagesReadRequest { ReaderId = bob.Id },
            CancellationToken.None);

        var messages = await db.Messages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();

        Assert.All(messages, m => Assert.NotNull(m.ReadAtUtc));
    }

    [Fact]
    public async Task GetConversations_ForAUserWithNoConversations_ReturnsEmptyList()
    {
        await using var db = CreateContext();
        var alice = AddUser(db, "alice@example.com");
        await db.SaveChangesAsync();

        var controller = new MessagingController(db);
        var result = await controller.GetConversations(alice.Id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var conversations = Assert.IsType<List<ConversationSummaryDto>>(okResult.Value);
        Assert.Empty(conversations);
    }

    [Fact]
    public async Task GetConversations_ForANonExistentUser_ReturnsNotFound()
    {
        await using var db = CreateContext();

        var controller = new MessagingController(db);
        var result = await controller.GetConversations(999, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}
