namespace Outty.Shared.Models.Messaging;

public class ConversationSummaryDto
{
    public int ConversationId { get; set; }

    public int OtherUserId { get; set; }

    public string OtherUserName { get; set; } = string.Empty;

    public string? LastMessage { get; set; }

    public DateTime? LastMessageAtUtc { get; set; }

    public int UnreadCount { get; set; }

    public bool HasUnreadMessages => UnreadCount > 0;

    public string LastMessageTimeText
    {
        get
        {
            if (!LastMessageAtUtc.HasValue)
            {
                return string.Empty;
            }

            var localTime = LastMessageAtUtc.Value.ToLocalTime();

            if (localTime.Date == DateTime.Today)
            {
                return localTime.ToString("h:mm tt");
            }

            if (localTime.Date == DateTime.Today.AddDays(-1))
            {
                return "Yesterday";
            }

            return localTime.ToString("MMM d");
        }
    }
}