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
    
    //private readonly UsersDomain _usersDomain; // vamos prossivelmente precisar de ter um creator domain para gerenciar o  intervalo de valores para campos etc (lógica de negócio) 
    private readonly ICreatorService _creatorService;

    public CreatorController(/*UsersDomain usersDomain,*/ ICreatorService creatorService)
    {
        //_usersDomain = usersDomain;
        _creatorService = creatorService;
    }

    [HttpPost(Uris.Uris.Creators.Create)] 
    public async Task<IActionResult> Create([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] CreatorCreateInputModel input)
    {
        var res = await _creatorService.Register (user.Id, input.ArtisticName);
            
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
}

//verificações de pipelina(ou handler), verificar se o user está registado pura e exclusivamente como creator ver se o token -> user -> creator(fazer get para obter creator desse user)

