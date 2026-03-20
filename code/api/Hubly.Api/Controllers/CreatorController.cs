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

public class CreatorController : ControllerBase
{

    //private readonly CreatorsDomain _creatorsDomain; 
    private readonly ICreatorService _creatorService;

    public CreatorController(/*CreatorsDomain creatorsDomain,*/ ICreatorService creatorService)
    {
        //_creatorsDomain = creatorsDomain;
        _creatorService = creatorService;
    }

    [HttpPost(Uris.Uris.Creators.Create)]
    public async Task<IActionResult> Create([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] CreatorCreateInputModel input)
    {
        var res = await _creatorService.Register(user.Id, input.ArtisticName);

        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Create), success.Adapt<CreatorCreateOutputModel>()),
            error => error switch
            {
                CreatorError.InvalidArtisticName => ProblemResponse.InvalidArtisticName.ToResponse(),
                CreatorError.CreatorAlreadyExists => ProblemResponse.CreatorAlreadyExists.ToResponse(),
                CreatorError.UserAlreadyRegisteredAsCompany => ProblemResponse.UserAlreadyRegisteredAsCompany.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );

    }

    [HttpPost(Uris.Uris.Creators.ChangeAvailabilityStatus)]
    public async Task<IActionResult> ChangeAvailabilityStatus([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] StatusChangeInputModel input)
    {
        // 1. Chamamos o método de atualização, passando o User do token e o novo status
        var res = await _creatorService.UpdateStatus(user.Id, input.AvailabilityStatus);

        return res.Match<IActionResult>(
            // 2. Se for sucesso, retornamos 200 OK ou 204 No Content
            success => Ok(success.Adapt<StatusChangeOutpuModel>()),
            // 3. Tratamos os erros específicos desta operação
            error => error switch
            {
                CreatorError.InvalidAvailabilityStatus => ProblemResponse.InvalidStatus.ToResponse(),
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Creators.RateCreator)]
    public async Task<IActionResult> RateCreator(
   [FromRoute] int id,
   [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
   [FromBody] RateCreatorInputModel request)
    {
        if (user.Id == id)
        {
            return ProblemResponse.SelfRatingNotAllowed.ToResponse();
        }

        var response = await _creatorService.RateCreator(id, request.Rate);

        return response.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                CreatorError.InvalidRating => ProblemResponse.InvalidRating.ToResponse(),
                CreatorError.ErrorRatingCreator => ProblemResponse.ErrorRatingCreator.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );

    }
}

//verificações de pipelina(ou handler), verificar se o user está registado pura e exclusivamente como creator ver se o token -> user -> creator(fazer get para obter creator desse user)

