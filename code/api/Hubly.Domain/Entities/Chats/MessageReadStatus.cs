namespace Hubly.api.Domain.Entities;

public class MessageReadStatus
{
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int? LastReadMessageId { get; set; }
    public Message? LastReadMessage { get; set; }

    public long? LastReadAt { get; set; }
}
