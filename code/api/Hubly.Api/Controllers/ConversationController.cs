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

    [HttpPost(Uris.Uris.Conversations.Create)]
    public async Task<IActionResult> Create(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, 
        [FromBody] CreateConversationInputModel input)
    {
        int? senderCompanyId = input.Sender.Type == ParticipantType.Company ? input.Sender.ProfileId : null;
        int? senderSocialProfileId = input.Sender.Type == ParticipantType.SocialProfile ? input.Sender.ProfileId : null;

        int? receiverCompanyId = input.Receiver.Type == ParticipantType.Company ? input.Receiver.ProfileId : null;
        int? receiverSocialProfileId = input.Receiver.Type == ParticipantType.SocialProfile ? input.Receiver.ProfileId : null;

        var res = await _conversationService.CreateConversation(user.Id, senderCompanyId, senderSocialProfileId, receiverCompanyId, receiverSocialProfileId);

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
    
}