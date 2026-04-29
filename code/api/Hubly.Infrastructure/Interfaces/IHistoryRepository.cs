using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IHistoryRepository
{
    Task<bool> AddView(ProfileViewHistory history);
    Task<List<Creator>> GetTopTrendingCreators(int limit);
    Task<List<Company>> GetTopTrendingCompanies(int limit);
    Task<List<ProfileViewHistory>> GetUserHistory(int userId, int limit = 20);
    Task<UserInterestProfile> GetUserInterests(int userId, int limit = 50);

}