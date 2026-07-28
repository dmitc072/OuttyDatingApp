namespace Outty.Shared.Models.Messaging;

public class SendMessageRequest
{
    public int SenderId { get; set; }

    public string Content { get; set; } = string.Empty;
}