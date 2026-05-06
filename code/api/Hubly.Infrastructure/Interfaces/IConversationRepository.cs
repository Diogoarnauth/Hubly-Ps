using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<int> AddConversation(Conversation conversation);
    Task<Conversation?> GetConversationByParticipants(int? sCompId, int? sProfId, int? rCompId, int? rProfId);    
    Task<bool> IsUserParticipant(int conversationId, int userId);
    Task<Conversation?> GetById(int id);
    Task Update(Conversation conversation);
    Task<List<Conversation>> GetCreatorConversationsByProfile(int userId, int socialProfileId);
    Task<List<Conversation>> GetConversationsByCompany(int userId, int companyId);
}