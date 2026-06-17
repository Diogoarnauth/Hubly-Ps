using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface ICoWorkerRepository
{
    Task<CoWorker?> GetCoWorker(int userId);
    Task<bool> InviteExists(int ownerId, string email);
    Task CreateInvite(int ownerId, string email);
    Task<CoWorkerInvite?> GetInviteById(int inviteId);
    Task UpdateStatus(int inviteId, string status);
    Task<List<CoWorkerInvite>> GetInvitesByEmail(string email);
    Task<List<CoWorkerInvite>> GetInvitesByOwner(int ownerId);
    Task CreateCoWorker(int userId, int ownerId);
    Task DeleteCoWorker(int userId);

    Task<CoWorker?> GetCoWorkerByOwnerAndUser(int ownerId, int coWorkerUserId);
    Task DeleteCoWorkerByIds(int ownerId, int coWorkerUserId);
    Task<List<CoWorker>> GetTeamByOwnerId(int ownerId);
}
