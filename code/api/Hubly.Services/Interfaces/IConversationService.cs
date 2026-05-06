using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IConversationService
    {
        Task<OneOf<int, ConversationError>> CreateConversation(int currentUserId, int? senderCompanyId, int? senderSocialProfileId, int? receiverCompanyId, int? receiverSocialProfileId);
        Task<OneOf<bool, ConversationError>> CheckConversationExists(int currentUserId, int? senderCompanyId, int? senderSocialProfileId, int? receiverCompanyId, int? receiverSocialProfileId);
        Task<OneOf<int, ConversationError>> SendMessage(int currentUserId, int conversationId, string content);
        Task<OneOf<bool, ConversationError>> EditMessage(int currentUserId, int messageId, string newContent);
        Task<OneOf<bool, ConversationError>> DeleteMessage(int currentUserId, int messageId);
        Task<OneOf<PagedResponse<Message>, ConversationError>> GetMessages(int currentUserId,int conversationId,int page = 1,int pageSize = 25);
        Task<OneOf<List<ConversationWithLastMessage>, ConversationError>> GetCreatorConversationsByProfile(int userId, int socialProfileId);
        Task<OneOf<List<ConversationWithLastMessage>, ConversationError>> GetCompanyConversations(int userId, int companyId);
        Task<OneOf<bool, ConversationError>> MarkMessagesAsRead(int currentUserId, int conversationId, int lastMessageId);
        Task<OneOf<int, ConversationError>> GetUnreadMessageCount(int currentUserId, int conversationId);
    }
}   