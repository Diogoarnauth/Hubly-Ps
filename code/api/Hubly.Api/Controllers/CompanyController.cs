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
    
    private readonly ICompanyService _companyService;

    public CompanyController( ICompanyService companyService)
    {
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

     [HttpGet(Uris.Uris.Companies.GetById)] 
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var res = await _companyService.GetById(id);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<GetCompanyOutputModel>()), 
            error => error switch
            {
                CompanyError.CompanyNotFound => ProblemResponse.CompanyNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

}

//verificações de pipelina(ou handler), verificar se o user está registado pura e exclusivamente como creator ver se o token -> user -> creator(fazer get para obter creator desse user)

