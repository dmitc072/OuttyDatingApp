using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;

namespace Outty.Api.Services;

public record ConversationLookupResult(int ConversationId, bool WasCreated);

public class ConversationService(OuttyDbContext db)
{
    public async Task<ConversationLookupResult?> FindOrCreateConversationAsync(
        int userId1,
        int userId2,
        CancellationToken cancellationToken = default)
    {
        if (userId1 == userId2)
        {
            return null;
        }

        var usersExist = await db.Users
            .CountAsync(u => u.Id == userId1 || u.Id == userId2, cancellationToken);

        if (usersExist != 2)
        {
            return null;
        }

        var existingConversationId = await db.Conversations
            .Where(c =>
                c.ConversationParticipants.Count == 2 &&
                c.ConversationParticipants.Any(p => p.UserId == userId1) &&
                c.ConversationParticipants.Any(p => p.UserId == userId2))
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingConversationId.HasValue)
        {
            return new ConversationLookupResult(existingConversationId.Value, WasCreated: false);
        }

        var conversation = new Conversation
        {
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            ConversationParticipants =
            [
                new ConversationParticipant { UserId = userId1, JoinedAtUtc = DateTime.UtcNow },
                new ConversationParticipant { UserId = userId2, JoinedAtUtc = DateTime.UtcNow }
            ]
        };

        db.Conversations.Add(conversation);
        await db.SaveChangesAsync(cancellationToken);

        return new ConversationLookupResult(conversation.Id, WasCreated: true);
    }
}
