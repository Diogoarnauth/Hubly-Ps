using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IConversationService
    {
        Task<OneOf<int, ConversationError>> CreateConversation(int currentUserId, int? senderCompanyId, int? senderSocialProfileId, int? receiverCompanyId, int? receiverSocialProfileId);

    }
}   