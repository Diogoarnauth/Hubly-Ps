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

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost(Uris.Uris.Companies.Create)]
    public async Task<IActionResult> Create([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] CompanyInputModel input)
    {
        var res = await _companyService.Register(user.Id, input.CompanySize, input.CompanyName, input.Description, input.Sectors, input.WebsiteLink, input.CountryHeadquarters);

        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Create), success.Adapt<CompanyOutputModel>()),
            error => error switch
            {
                CompanyError.InvalidSectorName => ProblemResponse.InvalidSectorName.ToResponse(),
                CompanyError.InvalidArtisticName => ProblemResponse.InvalidArtisticName.ToResponse(),
                CompanyError.CompanyAlreadyExists => ProblemResponse.CompanyAlreadyExists.ToResponse(),
                CompanyError.UserAlreadyRegisteredAsCreator => ProblemResponse.UserAlreadyRegisteredAsCreator.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );

    }

    [HttpPost(Uris.Uris.Companies.EditCompanyProfile)]
    public async Task<IActionResult> EditProfile([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] CompanyInputModel input)
    {
        var res = await _companyService.EditProfile(user.Id, input.CompanySize, input.CompanyName, input.Description, input.Sectors, input.WebsiteLink, input.CountryHeadquarters);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<CompanyOutputModel>()),
            error => error switch
            {
                
                CompanyError.FailedToGetCompanyInfo => ProblemResponse.FailedToGetCompanyInfo.ToResponse(), 
                CompanyError.InvalidSectorName => ProblemResponse.InvalidSectorName.ToResponse(),
                CompanyError.InvalidWebSiteLink => ProblemResponse.InvalidWebSiteLink.ToResponse(),
                CompanyError.InvalidCountryHeadquarters => ProblemResponse.InvalidCountryHeadquarters.ToResponse(),
                CompanyError.UserAlreadyRegisteredAsCreator => ProblemResponse.UserAlreadyRegisteredAsCreator.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Companies.GetById)]
    public async Task<IActionResult> GetById( [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,[FromRoute] int id)
    {
        var res = await _companyService.GetById(id, user.Id);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<CompanyOutputModel>()),
            error => error switch
            {
                CompanyError.CompanyNotFound => ProblemResponse.CompanyNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }


    [HttpGet(Uris.Uris.Companies.Search)]
    public async Task<IActionResult> Search(
    [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
    [FromQuery] CompanySearchInputModel input)
    {
        var res = await _companyService.Search(
            input.Name,
            input.Sectors,
            input.CompanySize,
            input.Countries,
            input.Page,
            input.PageSize
        );

        return res.Match<IActionResult>(
            success => Ok(new
            {
                Items = success.Items.Adapt<List<CompanyOutputModel>>(),
                TotalItems = success.TotalItems,
                Page = success.Page,
                PageSize = success.PageSize
            }),
            error => error switch
            {
                CompanyError.FailedToGetCompanyInfo => ProblemResponse.FailedToGetCompanyInfo.ToResponse(),
                 _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

}

//verificações de pipelina(ou handler), verificar se o user está registado pura e exclusivamente como creator ver se o token -> user -> creator(fazer get para obter creator desse user)

