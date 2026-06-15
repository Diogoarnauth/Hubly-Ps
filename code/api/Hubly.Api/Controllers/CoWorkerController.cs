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
public class CoWorkerController : ControllerBase
{
    private readonly ICoWorkerService _coWorkerService;

    public CoWorkerController(ICoWorkerService coWorkerService)
    {
        _coWorkerService = coWorkerService;
    }

    [HttpPost(Uris.Uris.CoWorkers.SendInvite)]
    public async Task<IActionResult> SendInvite(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, 
        [FromBody] SendInviteInputModel input)
    {
        var res = await _coWorkerService.SendInvite(user.Id, input.Email);

        return res.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                CoWorkerError.UserIsNotACreatorOrCompany => ProblemResponse.UserIsNotACreatorOrCompany.ToResponse(),
                CoWorkerError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                CoWorkerError.CannotInviteSelf => ProblemResponse.CannotInviteSelf.ToResponse(),
                CoWorkerError.UserCannotBeACoWorker => ProblemResponse.UserCannotBeACoWorker.ToResponse(),
                CoWorkerError.UserAlreadyACoWorker => ProblemResponse.UserAlreadyACoWorker.ToResponse(),
                CoWorkerError.AlreadyInvited => ProblemResponse.AlreadyInvited.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.CoWorkers.AcceptInvite)]
    public async Task<IActionResult> AcceptInvite(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, 
        [FromRoute] int inviteId)
    {
        var res = await _coWorkerService.AcceptInvite(user.Id, inviteId);

        return res.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                CoWorkerError.InviteNotFound => ProblemResponse.InviteNotFound.ToResponse(),
                CoWorkerError.InviteExpired => ProblemResponse.InviteExpired.ToResponse(),
                CoWorkerError.Unauthorized => ProblemResponse.Unauthorized.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.CoWorkers.RejectInvite)]
    public async Task<IActionResult> RejectInvite(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, 
        [FromRoute] int inviteId)
    {
        var res = await _coWorkerService.RejectInvite(user.Id, inviteId);

        return res.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                CoWorkerError.InviteNotFound => ProblemResponse.InviteNotFound.ToResponse(),
                CoWorkerError.Unauthorized => ProblemResponse.Unauthorized.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.CoWorkers.GetReceivedInvites)]
    public async Task<IActionResult> GetReceivedInvites(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user)
    {
        var res = await _coWorkerService.GetReceivedInvites(user.Id);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<List<CoWorkerInviteOutputModel>>()),
            error => error switch
            {
                CoWorkerError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.CoWorkers.GetSentInvites)]
    public async Task<IActionResult> GetSentInvites(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user)
    {
        var res = await _coWorkerService.GetSentInvites(user.Id);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<List<CoWorkerInviteOutputModel>>()),
            error => error switch
            {
                CoWorkerError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }


    
    [HttpGet(Uris.Uris.CoWorkers.GetMyCoWorkerInfo)]
    public async Task<IActionResult> GetMyCoWorkerInfo( [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user)
    {
        var response = await _coWorkerService.GetMyCoWorkerInfo(user.Id);
        return response.Match<IActionResult>(
            success => Ok(success.Adapt<GetMyCoWorkerOutputModel>()),
            error => error switch
            {
                CoWorkerError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }
}