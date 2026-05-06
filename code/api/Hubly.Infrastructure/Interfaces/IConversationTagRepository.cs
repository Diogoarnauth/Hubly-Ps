using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IConversationTagRepository
{
    Task<ConversationTag?> GetById(int tagId);
    Task<List<ConversationTag>> GetUserTags(int userId);
    Task<int> CreateTag(ConversationTag tag);
    Task UpdateTag(ConversationTag tag);
    Task DeleteTag(int tagId);
    Task<ConversationTagAssignment?> GetAssignment(int userId, int conversationId);
    Task<List<ConversationTag>> GetConversationTags(int userId, int conversationId);
    Task AssignTag(ConversationTagAssignment assignment);
    Task RemoveTag(int userId, int conversationId);
    Task<bool> TagNameExistsForUser(int userId, string tagName);
}
