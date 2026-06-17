using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;

namespace Hubly.api.Services.Interfaces
{
    public interface ICoWorkerService
    {
        Task<OneOf<bool, CoWorkerError>> SendInvite(int ownerId, string email);
        Task<OneOf<bool, CoWorkerError>> AcceptInvite(int userId, int inviteId);
        Task<OneOf<bool, CoWorkerError>> RejectInvite(int userId, int inviteId);
        Task<OneOf<List<CoWorkerInvite>, CoWorkerError>> GetReceivedInvites(int userId);
        Task<OneOf<List<CoWorkerInvite>, CoWorkerError>> GetSentInvites(int userId);
        Task<OneOf<CoWorker, CoWorkerError>> GetMyCoWorkerInfo(int userId);
        Task<OneOf<bool, CoWorkerError>> CancelCoworking(int userId);
        Task<OneOf<bool, CoWorkerError>> OwnerCancelCoworking(int userId, int coWorkerUserId);
        Task<OneOf<List<CoWorker>, CoWorkerError>> GetMyTeam(int ownerId);


    }
}