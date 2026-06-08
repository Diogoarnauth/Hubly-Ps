using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IHistoryRepository
{
    Task<bool> AddView(ProfileViewHistory history);
    Task<List<CreatorSocialProfile>> GetTopTrendingCreators(int limit);
    Task<List<Company>> GetTopTrendingCompanies(int limit);
    Task<PagedResponse<ProfileViewHistory>> GetUserHistory(int userId, int page, int pageSize);
    Task<UserInterestProfile> GetUserInterests(int userId, int limit = 50);
    Task<CreatorInterestProfile> GetCreatorInterests(int userId, int limit = 50);


}