using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;

namespace Hubly.api.Services.Interfaces
{
    public interface ICoWorkerService
    {
        Task<OneOf<Success, CoWorkerError>> SendInvite(int ownerId, string email);
        Task<OneOf<Success, CoWorkerError>> AcceptInvite(int userId, int inviteId);
        Task<OneOf<Success, CoWorkerError>> RejectInvite(int userId, int inviteId);
        Task<OneOf<List<CoWorkerInvite>, Error>> GetReceivedInvites(int userId);
        Task<OneOf<List<CoWorkerInvite>, Error>> GetSentInvites(int userId);
    }
}