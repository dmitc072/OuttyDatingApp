using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class Conversation
{
    public int Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual ICollection<ConversationParticipant> ConversationParticipants
    {
        get;
        set;
    } = new List<ConversationParticipant>();

    public virtual ICollection<Message> Messages
    {
        get;
        set;
    } = new List<Message>();
}