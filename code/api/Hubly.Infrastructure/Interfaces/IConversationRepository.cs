using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<int> AddConversation(Conversation conversation);
    Task<Conversation?> GetConversationBetweenUsers(int userId1, int userId2);
    Task<bool> IsUserParticipant(int conversationId, int userId);
    Task<Conversation?> GetById(int id);
    Task Update(Conversation conversation);
}