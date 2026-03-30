using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface ICreatorService
    {
        Task<OneOf<Creator, CreatorError>> Register(int userId, string ArtisticName);
        Task<OneOf<Creator, CreatorError>> UpdateStatus(int userId, string newStatus);
        Task<OneOf<bool, CreatorError>> RateCreator(int creatorId, int rating);
        Task<OneOf<CreatorSocialProfile, CreatorError>> AddSocialProfile(int userId, string user_name, string link, int followers_count, decimal? priceMin, decimal? priceMax, int platform_id);
        Task<OneOf<bool, CreatorError>> RemoveSocialProfile(int userId, int profileId);
    }

}   
