using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<int> AddConversation(Conversation conversation);
    Task<Conversation?> GetConversationByParticipants(int? sCompId, int? sProfId, int? rCompId, int? rProfId);    
    Task<bool> IsUserParticipant(int conversationId, int userId);
    Task<Conversation?> GetById(int id);
    Task<Conversation?> GetConversationWithParticipants(int id);
    Task Update(Conversation conversation);
    Task<List<ConversationWithLastMessage>> GetCreatorConversationsByProfileExtended(int userId, int socialProfileId);
    Task<List<ConversationWithLastMessage>> GetCompanyConversationsExtended(int userId, int companyId);
    Task UpdateLastReadMessage(int conversationId, int userId, int messageId);
    Task<int?> GetLastReadMessageId(int conversationId, int userId);
    Task<int> GetUnreadMessageCount(int conversationId, int userId);
}