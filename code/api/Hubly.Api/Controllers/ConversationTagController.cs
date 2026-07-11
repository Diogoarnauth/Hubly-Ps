using Hubly.api.Domain.Entities;
using Hubly.api.DTOs;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Problems;
using Hubly.api.Pipeline;
using Microsoft.AspNetCore.Mvc;

namespace Hubly.api.Controllers;

[ApiController]
public class ConversationTagController : ControllerBase
{
    private readonly IConversationTagService _conversationTagService;

    public ConversationTagController(IConversationTagService conversationTagService)
    {
        _conversationTagService = conversationTagService;
    }

    [HttpPost(Uris.Uris.ConversationTags.CreateTag)] //LOG
   // [AuditLogFilter("CreateTag")]

    public async Task<IActionResult> CreateTag(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,         
        [FromBody] CreateConversationTagInputModel input)
    {
        var res = await _conversationTagService.CreateTag(user.Id, coWorker?.Id, input.TagName, input.ColorHex);

        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(CreateTag), new { id = success }, new { id = success }),
            error => error switch
            {
                ConversationTagError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                ConversationTagError.InvalidTagName => ProblemResponse.InvalidTagName.ToResponse(),
                ConversationTagError.InvalidColorHex => ProblemResponse.InvalidColorHex.ToResponse(),
                ConversationTagError.TagNameAlreadyExists => ProblemResponse.TagNameAlreadyExists.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.ConversationTags.GetUserTags)]
    public async Task<IActionResult> GetUserTags(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker)
    {
        var res = await _conversationTagService.GetUserTags(user.Id);

        return res.Match<IActionResult>(
            success => Ok(success.Select(ConversationTagOutputModel.FromEntity)),
            error => error switch
            {
                ConversationTagError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPut(Uris.Uris.ConversationTags.UpdateTag)] //LOG
    //[AuditLogFilter("UpdateTag")]
    public async Task<IActionResult> UpdateTag(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,
        int tagId,
        [FromBody] UpdateConversationTagInputModel input)
    {
        var res = await _conversationTagService.UpdateTag(user.Id, coWorker?.Id, tagId, input.TagName, input.ColorHex);

        return res.Match<IActionResult>(
            success => Ok(),
            error => error switch
            {
                ConversationTagError.TagNotFound => ProblemResponse.TagNotFound.ToResponse(),
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                ConversationTagError.InvalidTagName => ProblemResponse.InvalidTagName.ToResponse(),
                ConversationTagError.InvalidColorHex => ProblemResponse.InvalidColorHex.ToResponse(),
                ConversationTagError.TagNameAlreadyExists => ProblemResponse.TagNameAlreadyExists.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpDelete(Uris.Uris.ConversationTags.DeleteTag)] //LOG
    //[AuditLogFilter("DeleteTag")]
    public async Task<IActionResult> DeleteTag(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        int tagId)
    {
        var res = await _conversationTagService.DeleteTag(user.Id, coWorker?.Id, tagId);

        return res.Match<IActionResult>(
            success => Ok(),
            error => error switch
            {
                ConversationTagError.TagNotFound => ProblemResponse.TagNotFound.ToResponse(),
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.ConversationTags.TagConversation)] //LOG
    //[AuditLogFilter("TagConversation")]
    public async Task<IActionResult> TagConversation(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        int conversationId,
        [FromBody] TagConversationInputModel input)
    {
        var res = await _conversationTagService.TagConversation(user.Id, coWorker?.Id, conversationId, input.TagId);

        return res.Match<IActionResult>(
            success => Ok(new { success = true }),
            error => error switch
            {
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                ConversationTagError.TagNotFound => ProblemResponse.TagNotFound.ToResponse(),
                ConversationTagError.ConversationNotFound => ProblemResponse.ConversationNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.ConversationTags.UntagConversation)] //LOG
    //[AuditLogFilter("UntagConversation")]

    public async Task<IActionResult> UntagConversation(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker,
        int conversationId)
    {
        var res = await _conversationTagService.UntagConversation(user.Id, coWorker?.Id, conversationId);

        return res.Match<IActionResult>(
            success => Ok(new { success = true }),
            error => error switch
            {
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }
    
}
