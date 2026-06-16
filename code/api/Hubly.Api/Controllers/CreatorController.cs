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
                CreatorError.UserAlreadyRegisteredAsCoWorker => ProblemResponse.UserAlreadyRegisteredAsCoWorker.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );

    }

    [HttpPost(Uris.Uris.Creators.EditCreatorProfile)]
    public async Task<IActionResult> EditProfile(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker,
        [FromBody] CreatorCreateInputModel input)
    {
        var res = await _creatorService.Edit(user.Id, input.ArtisticName);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<CreatorCreateOutputModel>()),
            error => error switch
            {

                CreatorError.FailedToGetCreatorInfo => ProblemResponse.FailedToGetCreatorInfo.ToResponse(),
                CreatorError.UserAlreadyRegisteredAsCompany => ProblemResponse.UserAlreadyRegisteredAsCompany.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Creators.ChangeAvailabilityStatus)]
    public async Task<IActionResult> ChangeAvailabilityStatus(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker,         
        [FromBody] StatusChangeInputModel input)
    {
        var res = await _creatorService.UpdateStatus(user.Id, input.AvailabilityStatus);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<StatusChangeOutpuModel>()),
            error => error switch
            {
                CreatorError.InvalidAvailabilityStatus => ProblemResponse.InvalidStatus.ToResponse(),
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Creators.RateCreator)]
    public async Task<IActionResult> RateCreator([FromRoute] int id, [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] RateCreatorInputModel request)
    {
        if (user.Id == id)
        {
            return ProblemResponse.SelfRatingNotAllowed.ToResponse();
        }

        var response = await _creatorService.RateCreator(user.Id, id, request.Rate);

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

    [HttpGet(Uris.Uris.Creators.GetById)]
    public async Task<IActionResult> GetById(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int id)
    {
        var res = await _creatorService.GetById(id, user.Id);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<GetCreatorOutputModel>()),
            error => error switch
            {
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    //--- Social Platforms ---

    [HttpGet(Uris.Uris.Creators.GetSocialProfileById)]
    public async Task<IActionResult> GetSocialProfileById(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int profileId)
    {
        var res = await _creatorService.GetSocialProfileById(profileId, user.Id);

        return res.Match<IActionResult>(
            success =>
            {
                var dto = success.Profile.Adapt<GetSocialProfileOutputModel>();
                dto.IsOwner = success.IsOwner;
                return Ok(dto);
            },
            error => error switch
            {
                CreatorError.SocialProfileNotFound => ProblemResponse.SocialProfileNotFound.ToResponse(),
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }



    [HttpPost(Uris.Uris.Creators.AddSocialProfile)]
    public async Task<IActionResult> AddSocialProfile(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromBody] SocialProfileInputModel input)
    {
        var res = await _creatorService.AddSocialProfile(user.Id, input.Platform_user_name, input.Link, input.Description, input.Followers_count, input.PriceMin, input.PriceMax, input.PlatformId, input.Sectors);

        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Create), success.Adapt<GetSocialProfileOutputModel>()),
            error => error switch
            {
                CreatorError.InvalidArtisticName => ProblemResponse.InvalidArtisticName.ToResponse(),
                CreatorError.InvalidPriceRange => ProblemResponse.InvalidPriceRange.ToResponse(),
                CreatorError.InvalidWebSiteLink => ProblemResponse.InvalidWebSiteLink.ToResponse(),
                CreatorError.InvalidFollowersCount => ProblemResponse.InvalidFollowersCount.ToResponse(),
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                CreatorError.PlatformNotFound => ProblemResponse.PlatformNotFound.ToResponse(),
                CreatorError.SocialProfileAlreadyExists => ProblemResponse.SocialProfileAlreadyExists.ToResponse(),
                CreatorError.InvalidSectorName => ProblemResponse.InvalidSectorName.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );

    }


    [HttpPost(Uris.Uris.Creators.EditCreatorSocialProfile)]
    public async Task<IActionResult> EditCreatorSocialProfile(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker,  
        [FromRoute] int socialProfileId, 
        [FromBody] EditPlatformsInputModel input)
    {
        var res = await _creatorService.EditCreatorSocialProfile(user.Id, socialProfileId, input.PlatformUserName, input.Link, input.Description, input.FollowersCount, input.PriceMin, input.PriceMax, input.Sectors);

        return res.Match<IActionResult>(
            success => Ok(success.Adapt<GetSocialProfileOutputModel>()),
            error => error switch
            {
                CreatorError.InvalidPriceRange => ProblemResponse.InvalidPriceRange.ToResponse(),
                CreatorError.InvalidArtisticName => ProblemResponse.InvalidArtisticName.ToResponse(),
                CreatorError.InvalidWebSiteLink => ProblemResponse.InvalidWebSiteLink.ToResponse(),
                CreatorError.InvalidFollowersCount => ProblemResponse.InvalidFollowersCount.ToResponse(),
                CreatorError.UserAlreadyRegisteredAsCompany => ProblemResponse.UserAlreadyRegisteredAsCompany.ToResponse(),
                CreatorError.SocialProfileNotFound => ProblemResponse.SocialProfileNotFound.ToResponse(),
                CreatorError.ProfileDoesntBellongToYou => ProblemResponse.ProfileDoesntBellongToYou.ToResponse(),
                CreatorError.FailedToGetCreatorSocialProfileInfo => ProblemResponse.FailedToGetCreatorSocialProfileInfo.ToResponse(),
                CreatorError.InvalidSectorName => ProblemResponse.InvalidSectorName.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }


    [HttpDelete(Uris.Uris.Creators.RemoveSocialProfile)]
    public async Task<IActionResult> RemoveSocialProfile(
        [FromServices] AuthenticatedUser user, 
        [FromServices] AuthenticatedCoWorker? coWorker,             
        [FromRoute] int profileId)
    {
        var res = await _creatorService.RemoveSocialProfile(user.Id, profileId);


        return res.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                CreatorError.SocialProfileNotFound => ProblemResponse.SocialProfileNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );

    }

    [HttpGet(Uris.Uris.Sectors.GetAll)]
    public async Task<IActionResult> GetAllSectors()
    {
        var res = await _creatorService.GetAllSectors();

        return res.Match<IActionResult>(
            success => Ok(success.Select(s => new { s.Id, Name = s.SectorName })),
            error => ProblemResponse.SectorsNotFound.ToResponse()
        );
    }

    [HttpGet(Uris.Uris.Creators.GetMyRating)]
    public async Task<IActionResult> GetMyRating(
        [FromServices] AuthenticatedUser user,
        [FromServices] AuthenticatedCoWorker? coWorker, 
        [FromRoute] int id)
    {
        var res = await _creatorService.GetUserRatingForCreator(user.Id, id);

        return res.Match<IActionResult>(
            rating => Ok(new { rating }),
            error => error switch
            {
                CreatorError.CreatorNotFound => ProblemResponse.CreatorNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Creators.Search)]
    public async Task<IActionResult> Search(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        [FromQuery] CreatorSearchInputModel input)
    {
        var res = await _creatorService.Search(
            input.PlatformId,
            input.PlatformUserName,
            input.FollowersCountMin,
            input.FollowersCountMax,
            input.PriceMin,
            input.PriceMax,
            input.Sectors,
            input.Page,
            input.PageSize
        );

        return res.Match<IActionResult>(
            success => Ok(new
            {
                Items = success.Items.Adapt<List<GetSocialProfileOutputModel>>(),
                TotalItems = success.TotalItems,
                Page = success.Page,
                PageSize = success.PageSize
            }),
            error => error switch
            {
                CreatorError.FailedToGetCreatorInfo => ProblemResponse.FailedToGetCreatorInfo.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }



    [HttpGet(Uris.Uris.Creators.GetTrending)]
    public async Task<IActionResult> GetTrending([FromQuery] int limit = 15)
    {
        var result = await _creatorService.GetTrendingCreators(limit);

        return result.Match(
            profiles =>
            {
                var response = profiles.Select(p => new
                {
                    user_id = p.CreatorId,
                    socialProfile_id = p.Id,
                    PlatformUserName = p.PlatformUserName,
                    PlatformName = p.Platform?.NamePlatform, 
                    Description = p.Description,
                    ArtisticName = p.Creator?.ArtisticName 
                }).ToList();

                return Ok(response);
            },
            error => error switch
            {
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }


[HttpGet(Uris.Uris.Creators.GetRecommendations)]
public async Task<IActionResult> GetRecommendations(
    [FromServices] AuthenticatedUser user, 
    [FromServices] AuthenticatedCoWorker? coWorker   
    )
{
    var res = await _creatorService.GetRecommendedCreators(user.Id);

    return res.Match<IActionResult>(
        success => 
        {
            var output = success.Select(sp => {
                var dto = sp.Adapt<GetSocialProfileOutputModel>();
                dto.PlatformName = sp.Platform?.NamePlatform; 
                dto.Sectors = sp.Sectors?.Select(s => s.SectorName).ToList() ?? new();
                return dto;
            }).ToList();

            return Ok(output);
        },
        error => error switch
        {
            _ => ProblemResponse.InternalServerError.ToResponse()
        }
    );
}


}
//verificações de pipelina(ou handler), verificar se o user está registado pura e exclusivamente como creator ver se o token -> user -> creator(fazer get para obter creator desse user)

