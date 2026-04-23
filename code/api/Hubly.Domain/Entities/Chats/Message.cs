using Hubly.api.Domain.Entities;

namespace Hubly.Domain.Entities.Chats;

public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public long SentAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
}