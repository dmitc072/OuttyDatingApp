using System;

namespace Outty.Api.Data;

public partial class ConversationParticipant
{
    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAtUtc { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}