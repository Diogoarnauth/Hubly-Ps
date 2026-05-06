using Hubly.api.Domain.Entities;

namespace Hubly.api.Domain.Entities;


public class ConversationWithLastMessage
{
    public Conversation Conversation { get; set; }
    public Message? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}