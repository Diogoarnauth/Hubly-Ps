using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICreatorSocialRepository
    {
        Task Add(CreatorSocialProfile profile);
        Task<bool> HasProfileInPlatform(int userId, int platformId);
        Task<CreatorSocialProfile?> GetById(int profileId);
        void Delete(CreatorSocialProfile profile);
        Task<bool> ExistsByPlatformAndUsername(int platformId, string username);
        Task<CreatorSocialProfile?> EditCreatorSocialProfile(int userId, int socialProfileId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax, List<Sector> sectors);
        Task<PagedResponse<CreatorSocialProfile>> Search(int? platform_id, string? platform_user_name, int? followers_count_min, int? followers_count_max, decimal? price_min, decimal? price_max, List<string>? sectors, int page, int page_size);

    }   

}