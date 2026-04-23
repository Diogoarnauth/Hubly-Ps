using Hubly.Domain.Entities.Chats;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<int> AddConversation(Conversation conversation);
    Task<Conversation?> GetConversationBetweenUsers(int userId1, int userId2);


}