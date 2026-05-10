using Hubly.api.Domain.Entities;

namespace Hubly.api.Domain.Entities;

public class ConversationWithLastMessage
{
    public Conversation Conversation { get; set; }
    public Message? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public ConversationTag? Tag { get; set; }
    
    public string OtherPartyName { get; set; } = "Unknown";
    public int? PlatformId { get; set; }
}