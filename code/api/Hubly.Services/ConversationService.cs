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

                int senderUserId;
                if (senderCompanyId.HasValue)
                {
                    var company = await context.CompanyRepository.GetByUserId(senderCompanyId.Value);
                    if (company == null || company.Id != currentUserId)
                        return new ConversationError.InvalidParticipantRole();
                    senderUserId = company.Id;
                }
                else if (senderSocialProfileId.HasValue)
                {
                    var profile = await context.CreatorSocialRepository.GetById(senderSocialProfileId.Value);
                    if (profile == null || profile.CreatorId != currentUserId)
                        return new ConversationError.InvalidParticipantRole();
                    senderUserId = profile.Id;
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

                var existing = await context.ConversationRepository.GetConversationByParticipants(
                                    senderCompanyId,
                                    senderSocialProfileId,
                                    receiverCompanyId,
                                    receiverSocialProfileId
                                );
                Console.WriteLine($"Creating conversation between user {senderUserId} and user {targetUserId} result {existing?.Id}");

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

        public async Task<OneOf<bool, ConversationError>> CheckConversationExists(
     int currentUserId,
     int? senderCompanyId, int? senderSocialProfileId,
     int? receiverCompanyId, int? receiverSocialProfileId)
        {
            return await _transactionManager.Run<OneOf<bool, ConversationError>>(async (context) =>
            {
                // 1. Validação do Sender (Quem está a tentar verificar)
                if (senderCompanyId.HasValue)
                {
                    var company = await context.CompanyRepository.GetByUserId(senderCompanyId.Value);
                    // Verifica se a empresa existe e se pertence ao utilizador logado
                    if (company == null || company.Id != currentUserId)
                        return new ConversationError.InvalidParticipantRole();
                }
                else if (senderSocialProfileId.HasValue)
                {
                    var profile = await context.CreatorSocialRepository.GetById(senderSocialProfileId.Value);
                    // Verifica se o perfil existe e se pertence ao utilizador logado
                    if (profile == null || profile.CreatorId != currentUserId)
                        return new ConversationError.InvalidParticipantRole();
                }
                else return new ConversationError.InvalidParticipantRole();

                // 2. Validação do Target (Destinatário)
                if (receiverCompanyId.HasValue)
                {
                    var targetComp = await context.CompanyRepository.GetByUserId(receiverCompanyId.Value);
                    if (targetComp == null) return new ConversationError.UserNotFound();
                }
                else if (receiverSocialProfileId.HasValue)
                {
                    var targetProf = await context.CreatorSocialRepository.GetById(receiverSocialProfileId.Value);
                    if (targetProf == null) return new ConversationError.UserNotFound();
                }
                else return new ConversationError.UserNotFound();

                // 3. Busca tipada no Repositório
                var existing = await context.ConversationRepository.GetConversationByParticipants(
                    senderCompanyId,
                    senderSocialProfileId,
                    receiverCompanyId,
                    receiverSocialProfileId
                );

                return existing != null;
            });
        }

        internal record SendMessageResult(int MessageId, List<int> ParticipantProfileIds);

        public async Task<OneOf<int, ConversationError>> SendMessage(int currentUserId, int conversationId, string content)
        {
            Console.WriteLine($"Hubly: 0");
            var result = await _transactionManager.Run<OneOf<SendMessageResult, ConversationError>>(async (context) =>
            {
                Console.WriteLine($"Hubly: 1");

                var isParticipant = await context.ConversationRepository.IsUserParticipant(conversationId, currentUserId);
                if (!isParticipant) return new ConversationError.AccessDenied();

                try
                {
                    Console.WriteLine($"Hubly: 2");

                    var sentAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var message = new Message
                    {
                        ConversationId = conversationId,
                        SenderId = currentUserId,
                        Content = content,
                        SentAt = sentAt,
                        IsEdited = false,
                        IsDeleted = false
                    };

                    var messageId = await context.MessageRepository.AddMessage(message);

                    var conversation = await context.ConversationRepository.GetById(conversationId);
                    if (conversation != null)
                    {
                        conversation.LastMessageAt = sentAt;
                        await context.ConversationRepository.Update(conversation);
                    }

                    // Fetch participants with Include to load navigation property
                    var conversationWithParticipants = await context.ConversationRepository.GetConversationWithParticipants(conversationId);
                    var participants = conversationWithParticipants?.Participants
                        .Select(p => p.CompanyId ?? p.SocialProfileId ?? 0)
                        .Where(id => id != 0)
                        .ToList() ?? new List<int>();

                    Console.WriteLine($"Hubly: 3, {participants.Count} participants found in conversation {conversationId}");

                    return new SendMessageResult(messageId, participants);
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });

            if (result.IsT0)
            {

                Console.WriteLine($"Hubly: 4");

                var data = result.AsT0;
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                await _eventService.SendToTopic(
                    $"chat_{conversationId}",
                    "NewMessage",
                    new
                    {
                        id = data.MessageId,
                        ConversationId = conversationId,
                        isEdited = false,
                        senderId = currentUserId,
                        sentAt = now,
                        content = content,
                        type = "created"
                    }
                );

                Console.WriteLine($"Hubly: 5, data: {data.ParticipantProfileIds.Count}");

                foreach (var profileId in data.ParticipantProfileIds)
                {
                    Console.WriteLine($"Hubly: Enviando atualização de sidebar para perfil {profileId} sobre nova mensagem na conversa {conversationId}");
                    await _eventService.SendToTopic(
                        $"all_conversations_topic_{profileId}",
                        "SidebarUpdate",
                        new
                        {
                            conversationId = conversationId,
                            content = content,
                            sentAt = now,
                            senderId = currentUserId,
                            type = "MESSAGE_CREATE"
                        }
                    );
                }

                return data.MessageId;
            }

            return result.AsT1;
        }

        public async Task<OneOf<bool, ConversationError>> EditMessage(int currentUserId, int messageId, string newContent)
        {
            var result = await _transactionManager.Run<OneOf<(int ConversationId, List<int> ParticipantProfileIds), ConversationError>>(async (context) =>
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

                    var conversation = await context.ConversationRepository.GetConversationWithParticipants(message.ConversationId);
                    var participants = conversation?.Participants
                        .Select(p => p.CompanyId ?? p.SocialProfileId ?? 0)
                        .Where(id => id != 0)
                        .ToList() ?? new List<int>();

                    return (message.ConversationId, participants);
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });

            if (result.IsT0)
            {
                var (convId, participantProfileIds) = result.AsT0;

                await _eventService.SendToTopic(
                    $"chat_{convId}",
                    "MessageUpdated",
                    new { id = messageId, content = newContent, isEdited = true }
                );

                foreach (var profileId in participantProfileIds)
                {
                    await _eventService.SendToTopic(
                        $"all_conversations_topic_{profileId}",
                        "SidebarUpdate",
                        new
                        {
                            conversationId = convId,
                            content = newContent,
                            sentAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            senderId = currentUserId,
                            type = "MESSAGE_EDIT"
                        }
                    );
                }

                return true;
            }

            return result.AsT1;
        }

        public async Task<OneOf<bool, ConversationError>> DeleteMessage(int currentUserId, int messageId)
        {
            var result = await _transactionManager.Run<OneOf<(int ConversationId, List<int> ParticipantProfileIds), ConversationError>>(async (context) =>
            {
                var message = await context.MessageRepository.GetById(messageId);

                if (message == null) return new ConversationError.MessageNotFound();
                if (message.SenderId != currentUserId) return new ConversationError.AccessDenied();

                try
                {
                    message.IsDeleted = true;
                    await context.MessageRepository.UpdateMessage(message);

                    var conversation = await context.ConversationRepository.GetConversationWithParticipants(message.ConversationId);
                    var participants = conversation?.Participants
                        .Select(p => p.CompanyId ?? p.SocialProfileId ?? 0)
                        .Where(id => id != 0)
                        .ToList() ?? new List<int>();

                    return (message.ConversationId, participants);
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });

            if (result.IsT0)
            {
                var (convId, participantProfileIds) = result.AsT0;

                await _eventService.SendToTopic(
                    $"chat_{convId}",
                    "MessageUpdated",
                    new { id = messageId, isDeleted = true }
                );

                foreach (var profileId in participantProfileIds)
                {
                    Console.WriteLine($"Hubly: Enviando atualização de sidebar para perfil {profileId} sobre mensagem deletada na conversa {convId}");
                    await _eventService.SendToTopic(
                        $"all_conversations_topic_{profileId}",
                        "SidebarUpdate",
                        new
                        {
                            conversationId = convId,
                            isDeleted = true,
                            senderId = currentUserId,
                            type = "MESSAGE_DELETE"
                        }
                    );
                }

                return true;
            }

            return result.AsT1;
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


        public async Task<OneOf<List<ConversationWithLastMessage>, ConversationError>> GetCreatorConversationsByProfile(int userId, int socialProfileId)
        {
            return await _transactionManager.Run<OneOf<List<ConversationWithLastMessage>, ConversationError>>(async (context) =>
            {
                var profile = await context.CreatorSocialRepository.GetById(socialProfileId);
                if (profile == null || profile.CreatorId != userId) return new ConversationError.AccessDenied();

                var conversations = await context.ConversationRepository.GetCreatorConversationsByProfile(userId, socialProfileId);
                Console.WriteLine($"Hubly: Found {conversations.Count} conversations for user {userId} and profile {socialProfileId}");

                var result = new List<ConversationWithLastMessage>();
                
                foreach (var conv in conversations)
                {
                    Console.WriteLine($"Hubly: Processing conversation {conv.Id} for profile {socialProfileId}");
                    var lastMsg = await context.MessageRepository.GetLastMessageByConversation(conv.Id);
                    var unreadCount = await context.ConversationRepository.GetUnreadMessageCount(conv.Id, userId);
                    var tag = await context.ConversationTagRepository.GetAssignment(userId, conv.Id);
                    Console.WriteLine($"vjdkkdd {tag.ConversationTag}");

                    result.Add(new ConversationWithLastMessage
                    {
                        Conversation = conv,
                        LastMessage = lastMsg,
                        UnreadCount = unreadCount,
                        Tag = tag?.ConversationTag
                    });

                    Console.WriteLine($"Hubly: Conversation {conv.Id} - LastMessageId: {lastMsg?.Id}, UnreadCount: {unreadCount}");
                }

                return result;
            });
        }


        public async Task<OneOf<List<ConversationWithLastMessage>, ConversationError>> GetCompanyConversations(int userId, int companyId)
        {
            return await _transactionManager.Run<OneOf<List<ConversationWithLastMessage>, ConversationError>>(async (context) =>
            {
                var company = await context.CompanyRepository.GetByUserId(companyId);
                if (company == null || company.Id != userId) return new ConversationError.AccessDenied();

                var conversations = await context.ConversationRepository.GetConversationsByCompany(userId, companyId);

                var result = new List<ConversationWithLastMessage>();

                foreach (var conv in conversations)
                {
                    var lastMsg = await context.MessageRepository.GetLastMessageByConversation(conv.Id);
                    var unreadCount = await context.ConversationRepository.GetUnreadMessageCount(conv.Id, userId);
                    var tag = await context.ConversationTagRepository.GetAssignment(userId, conv.Id);

                    result.Add(new ConversationWithLastMessage
                    {
                        Conversation = conv,
                        LastMessage = lastMsg,
                        UnreadCount = unreadCount,
                        Tag = tag.ConversationTag
                    });
                }

                return result;
            });
        }
        public async Task<OneOf<bool, ConversationError>> MarkMessagesAsRead(int currentUserId, int conversationId, int lastMessageId)
        {
            var result = await _transactionManager.Run<OneOf<int, ConversationError>>(async (context) =>
            {
                var conversation = await context.ConversationRepository.GetConversationWithParticipants(conversationId);
                if (conversation == null) return new ConversationError.InternalError(); 

                var participant = conversation.Participants
                    .FirstOrDefault(p => p.UserId == currentUserId);

                if (participant == null) return new ConversationError.AccessDenied();

                var message = await context.MessageRepository.GetById(lastMessageId);
                if (message == null || message.ConversationId != conversationId)
                {
                    return new ConversationError.MessageNotFound();
                }

                try
                {
                    await context.ConversationRepository.UpdateLastReadMessage(conversationId, currentUserId, lastMessageId);

                    int targetProfileId = participant.SocialProfileId ?? participant.CompanyId ?? 0;

                    return targetProfileId;
                }
                catch (Exception)
                {
                    return new ConversationError.InternalError();
                }
            });

            if (result.IsT0)
            {
                var profileIdForTopic = result.AsT0;

                if (profileIdForTopic != 0)
                {
                    Console.WriteLine($"Hubly: Enviando atualização de sidebar para o perfil {profileIdForTopic} na conversa {conversationId}");

                    await _eventService.SendToTopic(
                        $"all_conversations_topic_{profileIdForTopic}",
                        "SidebarUpdate",
                        new
                        {
                            conversationId = conversationId,
                            lastReadMessageId = lastMessageId,
                            currentUserId = currentUserId,
                            type = "READ_UPDATE"
                        }
                    );
                }

                return true;
            }

            return result.AsT1;
        }
        public async Task<OneOf<int, ConversationError>> GetUnreadMessageCount(int currentUserId, int conversationId)
        {
            return await _transactionManager.Run<OneOf<int, ConversationError>>(async (context) =>
            {
                var isParticipant = await context.ConversationRepository.IsUserParticipant(conversationId, currentUserId);
                if (!isParticipant)
                {
                    return new ConversationError.AccessDenied();
                }

                var count = await context.ConversationRepository.GetUnreadMessageCount(conversationId, currentUserId);
                Console.WriteLine($"Hubly: Unread message count for user {currentUserId} in conversation {conversationId} is {count}");
                return count;
            });
        }
    }
}