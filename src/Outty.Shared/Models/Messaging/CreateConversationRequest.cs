namespace Outty.Shared.Models.Messaging;

public class CreateConversationRequest
{
    public int CurrentUserId { get; set; }

    public int OtherUserId { get; set; }
}