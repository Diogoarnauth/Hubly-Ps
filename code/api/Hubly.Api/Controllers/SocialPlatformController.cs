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

public class SocialPlatformController : ControllerBase
{

    //private readonly CreatorsDomain _creatorsDomain; 
    private readonly ISocialPlatformService _socialPlatformService;

    public SocialPlatformController( ISocialPlatformService socialPlatformService)
    {
        _socialPlatformService = socialPlatformService;
    }

    [HttpGet(Uris.Uris.SocialPlatforms.GetAll)]
    public async Task<IActionResult> GetAllPlatforms(AuthenticatedUser user)
    {
        var response = await _socialPlatformService.GetAllPlatforms();
        return response.Match<IActionResult>(
            success => Ok(success.Adapt<List<SocialPlatformsOutputModel>>()),
            error => error switch
            {
                SocialPlatformError.FailedToGetPlatforms => ProblemResponse.FailedToGetPlatforms.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }


}
