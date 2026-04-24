using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IConversationService
    {
        Task<OneOf<int, ConversationError>> CreateConversation(int currentUserId, int? senderCompanyId, int? senderSocialProfileId, int? receiverCompanyId, int? receiverSocialProfileId);
        Task<OneOf<int, ConversationError>> SendMessage(int currentUserId, int conversationId, string content);
        Task<OneOf<bool, ConversationError>> EditMessage(int currentUserId, int messageId, string newContent);
        Task<OneOf<bool, ConversationError>> DeleteMessage(int currentUserId, int messageId);
        Task<OneOf<PagedResponse<Message>, ConversationError>> GetMessages(int currentUserId,int conversationId,int page = 1,int pageSize = 25);
    }
}   