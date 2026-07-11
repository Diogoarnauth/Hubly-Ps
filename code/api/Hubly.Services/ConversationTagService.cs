using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Domain.Entities;
using OneOf;
using Hubly.api.Infrastructure.Audit;


namespace Hubly.api.Services;

public class ConversationTagService : IConversationTagService
{
    private readonly ITransactionManager _transactionManager;

    private readonly AuditQueue _auditQueue;

    public ConversationTagService(ITransactionManager transactionManager,AuditQueue auditQueue)
    {
        _transactionManager = transactionManager;
        _auditQueue = auditQueue;

    }

   public async Task<OneOf<int, ConversationTagError>> CreateTag(int userId, int? coWorkerId, string tagName, string colorHex)
    {
        return await _transactionManager.Run<OneOf<int, ConversationTagError>>(async (context) =>
        {
            // Validar user
            var user = await context.UserRepository.GetUserById(userId);
            if (user == null)
                return new ConversationTagError.UserNotFound();

            // Validar nome da tag
            if (string.IsNullOrWhiteSpace(tagName) || tagName.Length > 50)
                return new ConversationTagError.InvalidTagName();

            // Validar cor hex
            if (!IsValidHexColor(colorHex))
                return new ConversationTagError.InvalidColorHex();

            // Verificar se tag com este nome já existe para este user
            var tagNameExists = await context.ConversationTagRepository.TagNameExistsForUser(userId, tagName);
            if (tagNameExists)
                return new ConversationTagError.TagNameAlreadyExists();

            var tag = new ConversationTag
            {
                UserId = userId,
                TagName = tagName,
                ColorHex = colorHex,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var tagId = await context.ConversationTagRepository.CreateTag(tag);


             var myUser = await context.UserRepository.GetUserById(coWorkerId ?? userId);

                    await _auditQueue.EnqueueAsync(new AuditEntry(
                                    "CreateTag",
                                    new {  TagReference = tagName, UserEmail = myUser.Email},
                                    userId,
                                    coWorkerId
                                ));


            return tagId;
        });
    }

      public async Task<OneOf<bool, ConversationTagError>> UpdateTag(int userId, int? coWorkerId, int tagId, string tagName, string colorHex)
    {
        return await _transactionManager.Run<OneOf<bool, ConversationTagError>>(async (context) =>
        {
            var tag = await context.ConversationTagRepository.GetById(tagId);
            if (tag == null)
                return new ConversationTagError.TagNotFound();

            if (tag.UserId != userId)
                return new ConversationTagError.UnauthorizedAccess();

            if (string.IsNullOrWhiteSpace(tagName) || tagName.Length > 50)
                return new ConversationTagError.InvalidTagName();

            if (!IsValidHexColor(colorHex))
                return new ConversationTagError.InvalidColorHex();

            var tagNameExists = await context.ConversationTagRepository.TagNameExistsForUser(userId, tagName);
            if (tagNameExists)
            {
                var existingTag = await context.ConversationTagRepository.GetById(tagId);
                if (existingTag == null || existingTag.TagName.ToLower() != tagName.ToLower())
                    return new ConversationTagError.TagNameAlreadyExists();
            }

            tag.TagName = tagName;
            tag.ColorHex = colorHex;

            await context.ConversationTagRepository.UpdateTag(tag);

             var myUser = await context.UserRepository.GetUserById(coWorkerId ?? userId);

                    await _auditQueue.EnqueueAsync(new AuditEntry(
                                    "UpdateTag",
                                    new {TagReference = tagName, UserEmail = myUser.Email},
                                    userId,
                                    coWorkerId
                                ));

            return true;
        });
    }


   public async Task<OneOf<bool, ConversationTagError>> DeleteTag(int userId, int? coWorkerId, int tagId)
    {
        return await _transactionManager.Run<OneOf<bool, ConversationTagError>>(async (context) =>
        {
            var tag = await context.ConversationTagRepository.GetById(tagId);
            if (tag == null)
                return new ConversationTagError.TagNotFound();

            if (tag.UserId != userId)
                return new ConversationTagError.UnauthorizedAccess();

            await context.ConversationTagRepository.DeleteTag(tagId);

               var myUser = await context.UserRepository.GetUserById(coWorkerId ?? userId);

                    await _auditQueue.EnqueueAsync(new AuditEntry(
                                    "DeleteTag",
                                    new {TagReference = tag.TagName, UserEmail = myUser.Email},
                                    userId,
                                    coWorkerId
                                ));


            return true;
        });
    }

    public async Task<OneOf<List<ConversationTag>, ConversationTagError>> GetUserTags(int userId)
    {
        return await _transactionManager.Run<OneOf<List<ConversationTag>, ConversationTagError>>(async (context) =>
        {
            var user = await context.UserRepository.GetUserById(userId);
            if (user == null)
                return new ConversationTagError.UserNotFound();

            var tags = await context.ConversationTagRepository.GetUserTags(userId);
            return tags;
        });
    }

   
    public async Task<OneOf<bool, ConversationTagError>> TagConversation(int userId, int? coWorkerId, int conversationId, int tagId)
    {
        return await _transactionManager.Run<OneOf<bool, ConversationTagError>>(async (context) =>
        {
            var isParticipant = await context.ConversationRepository.IsUserParticipant(conversationId, userId);
            if (!isParticipant)
                return new ConversationTagError.UnauthorizedAccess();

            var tag = await context.ConversationTagRepository.GetById(tagId);
            if (tag == null)
                return new ConversationTagError.TagNotFound();

            if (tag.UserId != userId && tag.UserId != null)
                return new ConversationTagError.UnauthorizedAccess();

            var assignment = new ConversationTagAssignment
            {
                UserId = userId,
                ConversationId = conversationId,
                TagId = tagId,
                UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            await context.ConversationTagRepository.AssignTag(assignment);


            var myUser = await context.UserRepository.GetUserById(coWorkerId ?? userId);

            var conversationWithParticipants = await context.ConversationRepository.GetConversationWithParticipants(conversationId);
        
            var targetParticipant = conversationWithParticipants?.Participants?
                .FirstOrDefault(p => p.UserId != userId);

            var receiverName = targetParticipant?.User?.Name ?? "Utilizador Desconhecido";

            await _auditQueue.EnqueueAsync(new AuditEntry(
                            "AssignTag",
                            new {TagReference = tag.TagName, UserEmail = myUser.Email, ReceiverName = receiverName},
                            userId,
                            coWorkerId
                        ));

            return true;
        });
    }


     public async Task<OneOf<bool, ConversationTagError>> UntagConversation(int userId, int? coWorkerId, int conversationId)
    {
        return await _transactionManager.Run<OneOf<bool, ConversationTagError>>(async (context) =>
        {
            var isParticipant = await context.ConversationRepository.IsUserParticipant(conversationId, userId);
            if (!isParticipant)
                return new ConversationTagError.UnauthorizedAccess();

            await context.ConversationTagRepository.RemoveTag(userId, conversationId);


            var myUser = await context.UserRepository.GetUserById(coWorkerId ?? userId);

            var conversationWithParticipants = await context.ConversationRepository.GetConversationWithParticipants(conversationId);
        
            var targetParticipant = conversationWithParticipants?.Participants?
                .FirstOrDefault(p => p.UserId != userId);

            var receiverName = targetParticipant?.User?.Name ?? "Utilizador Desconhecido";

            await _auditQueue.EnqueueAsync(new AuditEntry(
                            "UntagConversation",
                            new {UserEmail = myUser.Email, ReceiverName = receiverName},
                            userId,
                            coWorkerId
                        ));
            return true;
        });
    }

    private bool IsValidHexColor(string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return false;

        if (!hexColor.StartsWith("#"))
            return false;

        if (hexColor.Length != 7)
            return false;

        return System.Text.RegularExpressions.Regex.IsMatch(hexColor, "^#[0-9A-Fa-f]{6}$");
    }
}
