using Hubly.api.Domain.Entities;
using Hubly.api.DTOs;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Problems;
using Hubly.api.Pipeline;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Hubly.api.Controllers;

[ApiController]

public class ConversationController : ControllerBase
{

    private readonly IConversationService _conversationService;

    public ConversationController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpPost(Uris.Uris.Conversations.Create)] //LOG
    //[AuditLogFilter("CreateConversation")] 

    public async Task<IActionResult> Create(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromBody] CreateConversationInputModel input)
    {
        int? senderCompanyId = input.Sender.Type == ParticipantType.Company ? input.Sender.ProfileId : null;
        int? senderSocialProfileId = input.Sender.Type == ParticipantType.SocialProfile ? input.Sender.ProfileId : null;

        int? receiverCompanyId = input.Receiver.Type == ParticipantType.Company ? input.Receiver.ProfileId : null;
        int? receiverSocialProfileId = input.Receiver.Type == ParticipantType.SocialProfile ? input.Receiver.ProfileId : null;

        var res = await _conversationService.CreateConversation(user.Id, coWorker?.Id, senderCompanyId, senderSocialProfileId, receiverCompanyId, receiverSocialProfileId);

        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Create), new { id = success }, new { id = success }),
            error => error switch
            {
                ConversationError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                ConversationError.ConversationAlreadyExists => ProblemResponse.ConversationAlreadyExists.ToResponse(),
                ConversationError.InvalidParticipantRole => ProblemResponse.InvalidParticipantRole.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Conversations.CheckExists)]
    public async Task<IActionResult> CheckExists(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,
        [FromBody] CreateConversationInputModel input)
    {
        int? senderCompanyId = input.Sender.Type == ParticipantType.Company ? input.Sender.ProfileId : null;
        int? senderSocialProfileId = input.Sender.Type == ParticipantType.SocialProfile ? input.Sender.ProfileId : null;

        int? receiverCompanyId = input.Receiver.Type == ParticipantType.Company ? input.Receiver.ProfileId : null;
        int? receiverSocialProfileId = input.Receiver.Type == ParticipantType.SocialProfile ? input.Receiver.ProfileId : null;

        var res = await _conversationService.CheckConversationExists(user.Id, senderCompanyId, senderSocialProfileId, receiverCompanyId, receiverSocialProfileId);

        return res.Match<IActionResult>(
            success => Ok(new { exists = success }),
            error => error switch
            {
                ConversationError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                ConversationError.InvalidParticipantRole => ProblemResponse.InvalidParticipantRole.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Conversations.SendMessage)] //LOG
    //[AuditLogFilter("SendMessage")] 

    public async Task<IActionResult> SendMessage(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int conversationId, 
        [FromBody] SendMessageInputModel input)
    {
        var res = await _conversationService.SendMessage(user.Id, coWorker?.Id, conversationId, input.Content);

        return res.Match<IActionResult>(
            success => Ok(new { messageId = success }),
            error => error switch
            {
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Conversations.EditMessage)] //LOG
    //[AuditLogFilter("EditMessage")] 
    public async Task<IActionResult> EditMessage(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int messageId, 
        [FromBody] SendMessageInputModel input)
    {
        var res = await _conversationService.EditMessage(user.Id, coWorker?.Id, messageId, input.Content);

        return res.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                ConversationError.MessageNotFound => ProblemResponse.MessageNotFound.ToResponse(),
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                ConversationError.MessageAlreadyDeleted => ProblemResponse.MessageAlreadyDeleted.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpDelete(Uris.Uris.Conversations.DeleteMessage)] //LOG
    //[AuditLogFilter("DeleteMessage")] 

    public async Task<IActionResult> DeleteMessage(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,         
        [FromRoute] int messageId)
    {
        var res = await _conversationService.DeleteMessage(user.Id, coWorker?.Id, messageId);

        return res.Match<IActionResult>(
            success => NoContent(), // 204 No Content é o ideal para Deletes com sucesso
            error => error switch
            {
                ConversationError.MessageNotFound => ProblemResponse.MessageNotFound.ToResponse(),
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }


    [HttpGet(Uris.Uris.Conversations.GetMessages)]
    public async Task<IActionResult> GetMessages(
     [FromServices] AuthenticatedUser user,
     [FromServices] AuthenticatedCoWorker? coWorker, 
     [FromRoute] int conversationId,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 25)
    {
        var res = await _conversationService.GetMessages(user.Id, conversationId, page, pageSize);

        return res.Match<IActionResult>(
            success =>
            {
                var response = new PagedResponse<MessageResponseDTO>
                {
                    Page = success.Page,
                    PageSize = success.PageSize,
                    TotalItems = success.TotalItems,
                    Items = success.Items.Select(m => m.Adapt<MessageResponseDTO>()).ToList()
                };
                return Ok(response);
            },
            error => error switch
            {
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Conversations.GetMyConversationsCreator)]
    public async Task<IActionResult> GetMyConversationsCreator(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int socialProfileId)
    {
        var result = await _conversationService.GetCreatorConversationsByProfile(user.Id, socialProfileId);

        return result.Match<IActionResult>(
           success => Ok(success.Select(s => new ConversationOutputModel
           {
               Id = s.Conversation.Id,
               LastMessage = s.LastMessage?.Content ?? "",

               LastMessageAt = s.Conversation.LastMessageAt,

               OtherPartyName = s.OtherPartyName,
               PlatformId = s.PlatformId,

               UnreadCount = s.UnreadCount,
               Tag = s.Tag != null ? ConversationTagOutputModel.FromEntity(s.Tag) : null
           }).ToList()),
            error => error switch
            {
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    } 

    [HttpGet(Uris.Uris.Conversations.GetCompanyConversations)]
    public async Task<IActionResult> GetCompanyConversations(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,     
        [FromRoute] int companyId)
    {
        var result = await _conversationService.GetCompanyConversations(user.Id, companyId);

        return result.Match<IActionResult>(
            success => Ok(success.Select(s => new ConversationOutputModel
            {
                Id = s.Conversation.Id,
                LastMessage = s.LastMessage?.Content ?? "",
                LastMessageAt = s.Conversation.LastMessageAt,

                OtherPartyName = s.OtherPartyName,
                PlatformId = s.PlatformId,

                UnreadCount = s.UnreadCount,
                Tag = s.Tag != null ? ConversationTagOutputModel.FromEntity(s.Tag) : null
            }).ToList()),
            error => error switch
            {
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Conversations.MarkMessagesAsRead)] //LOG
    //[AuditLogFilter("MessagesRead")] 

    public async Task<IActionResult> MarkMessagesAsRead(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,
        [FromRoute] int conversationId,
        [FromRoute] int lastMessageId)
    {
        Console.WriteLine($"Controller");
        var res = await _conversationService.MarkMessagesAsRead(user.Id,conversationId, lastMessageId);

        return res.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                ConversationError.MessageNotFound => ProblemResponse.MessageNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Conversations.GetUnreadMessageCount)]
    public async Task<IActionResult> GetUnreadMessageCount(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int conversationId)
    {
        var res = await _conversationService.GetUnreadMessageCount(user.Id, conversationId);

        return res.Match<IActionResult>(
            success => Ok(new { unreadCount = success }),
            error => error switch
            {
                ConversationError.AccessDenied => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }



}