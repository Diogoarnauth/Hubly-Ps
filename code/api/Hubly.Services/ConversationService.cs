using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Hubly.api.Infrastructure;
using Hubly.Domain.Entities.Chats;


namespace Hubly.api.Services
{
    public class ConversationService : IConversationService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICreatorSocialRepository _creatorSocialRepository;



        public ConversationService(
            ITransactionManager transactionManager,
            IConversationRepository conversationRepository
            //IUserRepository userRepository,
            //ICompanyRepository companyRepository,
            //ICreatorSocialRepository creatorSocialRepository
            )
        {
            _transactionManager = transactionManager;
            _conversationRepository = conversationRepository;
            //_userRepository = userRepository;
            //_companyRepository = companyRepository;
            //_creatorSocialRepository creatorSocialRepository;
        }



        public async Task<OneOf<int, ConversationError>> CreateConversation(int currentUserId, int? senderCompanyId, int? senderSocialProfileId, int? receiverCompanyId, int? receiverSocialProfileId)
        {
            return await _transactionManager.Run<OneOf<int, ConversationError>>(async (context) =>
            {
                if (senderCompanyId.HasValue)
                {
                    var company = await context.CompanyRepository.GetByUserId(senderCompanyId.Value);
                    if (company == null || company.Id != currentUserId)
                        return new ConversationError.InvalidParticipantRole();
                }
                else if (senderSocialProfileId.HasValue)
                {
                    var profile = await context.CreatorSocialRepository.GetById(senderSocialProfileId.Value);
                    if (profile == null || profile.CreatorId != currentUserId)
                        return new ConversationError.InvalidParticipantRole();
                }
                else return new ConversationError.InvalidParticipantRole();

                int targetUserId;
                if (receiverCompanyId.HasValue)
                {
                    var targetComp = await context.CompanyRepository.GetByUserId(receiverCompanyId.Value);
                    if (targetComp == null) return new ConversationError.UserNotFound();
                    targetUserId = targetComp.Id;
                }
                else if (receiverSocialProfileId.HasValue)
                {
                    var targetProf = await context.CreatorSocialRepository.GetById(receiverSocialProfileId.Value);
                    if (targetProf == null) return new ConversationError.UserNotFound();
                    targetUserId = targetProf.CreatorId;
                }
                else return new ConversationError.UserNotFound();

                var existing = await context.ConversationRepository.GetConversationBetweenUsers(currentUserId, targetUserId);
                if (existing != null) return new ConversationError.ConversationAlreadyExists();

                try
                {
                    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    var conversation = new Conversation
                    {
                        CreatedAt = now,
                        LastMessageAt = now,
                        Participants = new List<ConversationParticipant>
                        {
                        new ConversationParticipant
                        {
                            UserId = currentUserId,
                            CompanyId = senderCompanyId,
                            SocialProfileId = senderSocialProfileId
                        },
                        new ConversationParticipant
                        {
                            UserId = targetUserId,
                            CompanyId = receiverCompanyId,
                            SocialProfileId = receiverSocialProfileId
                        }
                        }
                    };

                    var id = await context.ConversationRepository.AddConversation(conversation);
                    return id;
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });
        }

    }

}