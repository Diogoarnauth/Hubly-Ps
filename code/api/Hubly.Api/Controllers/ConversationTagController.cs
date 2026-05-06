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

    [HttpPost(Uris.Uris.ConversationTags.CreateTag)]
    public async Task<IActionResult> CreateTag(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        [FromBody] CreateConversationTagInputModel input)
    {
        var res = await _conversationTagService.CreateTag(user.Id, input.TagName, input.ColorHex);

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
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user)
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

    [HttpGet(Uris.Uris.ConversationTags.GetConversationTags)]
    public async Task<IActionResult> GetConversationTags(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        int conversationId)
    {
        var res = await _conversationTagService.GetConversationTags(user.Id, conversationId);

        return res.Match<IActionResult>(
            success => Ok(success.Select(ConversationTagOutputModel.FromEntity)),
            error => error switch
            {
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPut(Uris.Uris.ConversationTags.UpdateTag)]
    public async Task<IActionResult> UpdateTag(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        int tagId,
        [FromBody] UpdateConversationTagInputModel input)
    {
        var res = await _conversationTagService.UpdateTag(user.Id, tagId, input.TagName, input.ColorHex);

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

    [HttpDelete(Uris.Uris.ConversationTags.DeleteTag)]
    public async Task<IActionResult> DeleteTag(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        int tagId)
    {
        var res = await _conversationTagService.DeleteTag(user.Id, tagId);

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

    [HttpPost(Uris.Uris.ConversationTags.TagConversation)]
    public async Task<IActionResult> TagConversation(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        int conversationId,
        [FromBody] TagConversationInputModel input)
    {
        var res = await _conversationTagService.TagConversation(user.Id, conversationId, input.TagId);

        return res.Match<IActionResult>(
            success => Ok(),
            error => error switch
            {
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                ConversationTagError.TagNotFound => ProblemResponse.TagNotFound.ToResponse(),
                ConversationTagError.ConversationNotFound => ProblemResponse.ConversationNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.ConversationTags.UntagConversation)]
    public async Task<IActionResult> UntagConversation(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        int conversationId)
    {
        var res = await _conversationTagService.UntagConversation(user.Id, conversationId);

        return res.Match<IActionResult>(
            success => Ok(),
            error => error switch
            {
                ConversationTagError.UnauthorizedAccess => ProblemResponse.AccessDenied.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }
}
