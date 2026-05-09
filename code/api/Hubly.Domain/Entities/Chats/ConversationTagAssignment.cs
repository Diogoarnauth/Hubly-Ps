namespace Hubly.api.Domain.Entities;

public class ConversationTagAssignment
{
    public int UserId { get; set; }
    public int ConversationId { get; set; }
    public int TagId { get; set; }
    public long UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public virtual Conversation? Conversation { get; set; }
    public virtual ConversationTag? ConversationTag { get; set; }
}
