using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface ICreatorService
    {
        Task<OneOf<Creator, CreatorError>> Register(int userId, string artisticName);
        Task<OneOf<Creator, CreatorError>> UpdateStatus(int userId, string newStatus);
        Task<OneOf<Creator, CreatorError>> GetById(int targetCreatorId, int viewerId);
        Task<OneOf<bool, CreatorError>> RateCreator(int evaluatorId, int creatorId, int rating);
        Task<OneOf<CreatorSocialProfile, CreatorError>> AddSocialProfile(int userId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax, int platform_id, List<String> sectors); 
        Task<OneOf<bool, CreatorError>> RemoveSocialProfile(int userId, int profileId);
        Task<OneOf<(CreatorSocialProfile Profile, bool IsOwner), CreatorError>> GetSocialProfileById(int creatorProfileId, int userId);
        Task<OneOf<CreatorSocialProfile, CreatorError>> EditCreatorSocialProfile(int userId, int socialProfileId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax, List<String> sectors);
        Task<OneOf<PagedResponse<CreatorSocialProfile>, CreatorError>> Search(int? platform_id, string? platform_user_name, int? followers_count_min, int? followers_count_max, decimal? price_min, decimal? price_max, List<string>? sectors, int page, int page_size);
        Task<OneOf<List<Sector>, CreatorError>> GetAllSectors();
        Task<OneOf<List<Creator>, CreatorError>> GetTrendingCreators(int limit);
        Task<OneOf<Creator, CreatorError>> Edit(int user_id, string artisticName);
        
    }

}   
