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

public class CompanyController : ControllerBase
{
    
    //private readonly UsersDomain _usersDomain; // vamos prossivelmente precisar de ter um creator domain para gerenciar o  intervalo de valores para campos etc (lógica de negócio) 
    private readonly ICompanyService _companyService;

    public CompanyController(/*UsersDomain usersDomain,*/ ICompanyService companyService)
    {
        //_usersDomain = usersDomain;
        _companyService = companyService;
    }

    [HttpPost(Uris.Uris.Companies.Create)] 
    public async Task<IActionResult> Create([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] CompanyCreateInputModel input)
    {
        var res = await _companyService.Register (user.Id, input.CompanySize, input.CompanyName, input.Description, input.Sector, input.WebsiteLink, input.CountryHeadquarters);
            
        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Create), success.Adapt<CompanyCreateOutputModel>()),
            error => error switch
            {
                CompanyError.InvalidArtisticName => ProblemResponse.InvalidArtisticName.ToResponse(),
                CompanyError.CompanyAlreadyExists => ProblemResponse.CompanyAlreadyExists.ToResponse(), 
                CompanyError.UserAlreadyRegisteredAsCreator => ProblemResponse.UserAlreadyRegisteredAsCreator.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
        
    }

    [HttpPost(Uris.Uris.Companies.EditCompanyProfile)] 
    public async Task<IActionResult> EditProfile([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] CompanyEditInputModel input)
    {
        var res = await _companyService.EditProfile(user.Id, input.CompanySize, input.CompanyName, input.Description, input.Sector, input.WebsiteLink, input.CountryHeadquarters);
            
        return res.Match<IActionResult>(
            success => Ok(success.Adapt<CompanyEditOutputModel>()),
            error => error switch
            {
                CompanyError.InvalidSectorName => ProblemResponse.InvalidName.ToResponse(),
                CompanyError.CompanyAlreadyExists => ProblemResponse.CompanyAlreadyExists.ToResponse(), 
                CompanyError.UserAlreadyRegisteredAsCreator => ProblemResponse.UserAlreadyRegisteredAsCreator.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );  
    }

}

//verificações de pipelina(ou handler), verificar se o user está registado pura e exclusivamente como creator ver se o token -> user -> creator(fazer get para obter creator desse user)

