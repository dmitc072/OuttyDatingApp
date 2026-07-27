using System;

namespace Outty.Api.Data;

public partial class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAtUtc { get; set; }

    public DateTime? ReadAtUtc { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}