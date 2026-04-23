using Hubly.api.Domain.Entities;

namespace Hubly.Domain.Entities.Chats;


public class Conversation
{
    public int Id { get; set; }
    public long CreatedAt { get; set; }
    public long LastMessageAt { get; set; } 

    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}