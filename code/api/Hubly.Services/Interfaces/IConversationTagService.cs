using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces;

public interface IConversationTagService
{
    Task<OneOf<int, ConversationTagError>> CreateTag(int userId, int? coWorkerId, string tagName, string colorHex);
    Task<OneOf<bool, ConversationTagError>> UpdateTag(int userId, int? coWorkerId, int tagId, string tagName, string colorHex);
    Task<OneOf<bool, ConversationTagError>> DeleteTag(int userId, int? coWorkerId, int tagId);
    Task<OneOf<List<ConversationTag>, ConversationTagError>> GetUserTags(int userId);
    Task<OneOf<bool, ConversationTagError>> TagConversation(int userId, int? coWorkerId, int conversationId, int tagId);
    Task<OneOf<bool, ConversationTagError>> UntagConversation(int userId, int? coWorkerId, int conversationId);
}
