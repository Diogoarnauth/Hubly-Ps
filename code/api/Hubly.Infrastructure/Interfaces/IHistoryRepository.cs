using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IHistoryRepository
{
    Task<bool> AddView(ProfileViewHistory history);
    Task<List<ProfileViewHistory>> GetUserHistory(int userId, int limit = 20);
}