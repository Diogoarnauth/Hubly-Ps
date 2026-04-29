using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Domain.Entities;
using OneOf;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Hubly.api.Infrastructure.Data;


namespace Hubly.api.Services
{
    public class ConversationService : IConversationService
    {
        private readonly ITransactionManager _transactionManager;

        private readonly IEventService _eventService;

        public ConversationService(
            ITransactionManager transactionManager,
            IEventService eventService
        )
        {
            _transactionManager = transactionManager;
            _eventService = eventService;
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



        public async Task<OneOf<int, ConversationError>> SendMessage(int currentUserId, int conversationId, string content)
        {
            var result = await _transactionManager.Run<OneOf<int, ConversationError>>(async (context) =>
            {
                var isParticipant = await context.ConversationRepository.IsUserParticipant(conversationId, currentUserId);
                if (!isParticipant) return new ConversationError.AccessDenied();

                try
                {
                    var message = new Message
                    {
                        ConversationId = conversationId,
                        SenderId = currentUserId,
                        Content = content,
                        SentAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        IsEdited = false,
                        IsDeleted = false
                    };

                    var messageId = await context.MessageRepository.AddMessage(message);

                    var conversation = await context.ConversationRepository.GetById(conversationId);
                    if (conversation != null)
                    {
                        conversation.LastMessageAt = message.SentAt;
                        await context.ConversationRepository.Update(conversation);
                    }

                    return messageId;
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });
            if (result.IsT0) 
            {
                await _eventService.SendToTopic(
                    $"chat_{conversationId}", 
                    "NewMessage", 
                    new { 
                        id = result.AsT0, 
                        ConversationId = conversationId,
                        isEdited= false,
                        senderId = currentUserId, 
                        sentAt= DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        Content = content, 
                        Type = "created" 
                    }
                );
            }
            
            return result;
        }

        public async Task<OneOf<bool, ConversationError>> EditMessage(int currentUserId, int messageId, string newContent)
        {
            return await _transactionManager.Run<OneOf<bool, ConversationError>>(async (context) =>
            {
                var message = await context.MessageRepository.GetById(messageId);

                if (message == null) return new ConversationError.MessageNotFound();

                if (message.SenderId != currentUserId) return new ConversationError.AccessDenied();

                if (message.IsDeleted) return new ConversationError.MessageAlreadyDeleted();

                try
                {
                    message.Content = newContent;
                    message.IsEdited = true;

                    await context.MessageRepository.UpdateMessage(message);
                    return true;
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });
        }

        public async Task<OneOf<bool, ConversationError>> DeleteMessage(int currentUserId, int messageId)
        {
            return await _transactionManager.Run<OneOf<bool, ConversationError>>(async (context) =>
            {
                var message = await context.MessageRepository.GetById(messageId);

                if (message == null) return new ConversationError.MessageNotFound();

                if (message.SenderId != currentUserId)
                    return new ConversationError.AccessDenied();

                try
                {
                    message.IsDeleted = true;
                    await context.MessageRepository.UpdateMessage(message);

                    return true;
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });
        }

        public async Task<OneOf<PagedResponse<Message>, ConversationError>> GetMessages(int currentUserId, int conversationId, int page = 1, int pageSize = 25)
        {
            return await _transactionManager.Run<OneOf<PagedResponse<Message>, ConversationError>>(async (context) =>
            {

                var isParticipant = await context.ConversationRepository.IsUserParticipant(conversationId, currentUserId);
                if (!isParticipant) return new ConversationError.AccessDenied();

                try
                {
                    var pagedMessages = await context.MessageRepository.GetMessages(conversationId, page, pageSize);
                    return pagedMessages;
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });
        }

    }


}