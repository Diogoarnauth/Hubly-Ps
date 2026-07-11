using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;

namespace Hubly.api.Services
{
    public class CoWorkerService : ICoWorkerService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEmailService _emailService;

        public CoWorkerService(
            ITransactionManager transactionManager,
            IEmailService emailService
        )
        {
            _transactionManager = transactionManager;
            _emailService = emailService;
        }

        public async Task<OneOf<bool, CoWorkerError>> SendInvite(int ownerId, string email)
        {
            return await _transactionManager.Run<OneOf<bool, CoWorkerError>>(async (context) =>
            {

                var ownerUser = await context.UserRepository.GetUserById(ownerId);
                if (ownerUser == null) return new CoWorkerError.UserNotFound();
                if (ownerUser.Creator == null && ownerUser.Company == null)
                {
                    return new CoWorkerError.UserIsNotACreatorOrCompany();
                }


                var targetUser = await context.UserRepository.GetUserByEmail(email);
                if (targetUser == null) return new CoWorkerError.UserNotFound();

                if (targetUser.Id == ownerId) return new CoWorkerError.CannotInviteSelf();

                var fullTargetUser = await context.UserRepository.GetUserById(targetUser.Id);
                Console.WriteLine($"Target user: {fullTargetUser?.Name}, Creator: {fullTargetUser?.Creator}, Company: {fullTargetUser?.Company}");


                if (fullTargetUser == null) return new CoWorkerError.UserNotFound();
                if (fullTargetUser.Creator != null || fullTargetUser.Company != null)
                {
                    return new CoWorkerError.UserCannotBeACoWorker();
                }


                var existingCoWorker = await context.CoWorkerRepository.GetCoWorker(fullTargetUser.Id);
                if (existingCoWorker != null) return new CoWorkerError.UserAlreadyACoWorker();


                if (await context.CoWorkerRepository.InviteExists(ownerId, email))
                {
                    return new CoWorkerError.AlreadyInvited();
                }


                await context.CoWorkerRepository.CreateInvite(ownerId, email);

                await _emailService.SendCoWorkerInviteEmail(email, ownerUser.Name, ownerUser.Email);

                return true;
            });
        }


        public async Task<OneOf<bool, CoWorkerError>> AcceptInvite(int userId, int inviteId)
        {
            return await _transactionManager.Run<OneOf<bool, CoWorkerError>>(async (context) =>
            {
                var invite = await context.CoWorkerRepository.GetInviteById(inviteId);

                if (invite == null || invite.CoWorkerEmail != (await context.UserRepository.GetUserById(userId))?.Email)
                {
                    return new CoWorkerError.InviteNotFound();
                }

                if (invite.Status != "WAITING") return new CoWorkerError.Unauthorized();

                if (invite.ExpiresAt <= DateTime.UtcNow) return new CoWorkerError.InviteExpired();

                await context.CoWorkerRepository.CreateCoWorker(userId, invite.OwnerId);

                await context.CoWorkerRepository.UpdateStatus(inviteId, "ACCEPTED");

                return true;
            });
        }

        public async Task<OneOf<bool, CoWorkerError>> RejectInvite(int userId, int inviteId) //verificar validade do invite TIMESTAMP
        {
            return await _transactionManager.Run<OneOf<bool, CoWorkerError>>(async (context) =>
            {
                var invite = await context.CoWorkerRepository.GetInviteById(inviteId);

                if (invite == null || invite.CoWorkerEmail != (await context.UserRepository.GetUserById(userId))?.Email)
                {
                    return new CoWorkerError.InviteNotFound();
                }

                if (invite.Status != "WAITING") return new CoWorkerError.Unauthorized();

                await context.CoWorkerRepository.UpdateStatus(inviteId, "REJECTED");
                return true;
            });
        }

        public async Task<OneOf<List<CoWorkerInvite>, CoWorkerError>> GetReceivedInvites(int userId)
        {
            return await _transactionManager.Run<OneOf<List<CoWorkerInvite>, CoWorkerError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                if (user == null) return new CoWorkerError.UserNotFound();

                return await context.CoWorkerRepository.GetInvitesByEmail(user.Email);
            });
        }

        public async Task<OneOf<List<CoWorkerInvite>, CoWorkerError>> GetSentInvites(int userId)
        {
            return await _transactionManager.Run<OneOf<List<CoWorkerInvite>, CoWorkerError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                if (user == null) return new CoWorkerError.UserNotFound();

                return await context.CoWorkerRepository.GetInvitesByOwner(userId);
            });
        }

        public async Task<OneOf<CoWorker, CoWorkerError>> GetMyCoWorkerInfo(int userId)
        {
            return await _transactionManager.Run<OneOf<CoWorker, CoWorkerError>>(async (context) =>
            {
                var coWorker = await context.CoWorkerRepository.GetCoWorker(userId);

                if (coWorker == null)
                {
                    return new CoWorkerError.UserNotFound();
                }

                return coWorker;
            });
        }


        public async Task<OneOf<bool, CoWorkerError>> CancelCoworking(int userId)
        {
            return await _transactionManager.Run<OneOf<bool, CoWorkerError>>(async (context) =>
            {
                var coWorker = await context.CoWorkerRepository.GetCoWorker(userId);

                if (coWorker == null)
                {
                    return new CoWorkerError.CoWorkerRelationshipNotFound();
                }

                await context.CoWorkerRepository.DeleteCoWorker(userId);

                await context.CoWorkerRepository.DeleteAcceptedInvite(coWorker.OwnerId, coWorker.User.Email);

                return true;
            });
        }

        public async Task<OneOf<bool, CoWorkerError>> OwnerCancelCoworking(int ownerId, int coWorkerUserId)
        {
            return await _transactionManager.Run<OneOf<bool, CoWorkerError>>(async (context) =>
            {
                Console.WriteLine($"Owner ID: {ownerId}, CoWorker User ID: {coWorkerUserId}");
                var coWorker = await context.CoWorkerRepository.GetCoWorkerByOwnerAndUser(ownerId, coWorkerUserId);
                Console.WriteLine($"CoWorker relationship: {coWorker?.UserId} - {coWorker?.OwnerId}");

                if (coWorker == null)
                {
                    return new CoWorkerError.CoWorkerRelationshipNotFound();
                }

                await context.CoWorkerRepository.DeleteCoWorkerByIds(ownerId, coWorkerUserId);

                return true;
            });
        }

        public async Task<OneOf<List<CoWorker>, CoWorkerError>> GetMyTeam(int ownerId)
        {
            return await _transactionManager.Run<OneOf<List<CoWorker>, CoWorkerError>>(async (context) =>
            {
                var ownerUser = await context.UserRepository.GetUserById(ownerId);
                if (ownerUser == null) return new CoWorkerError.UserNotFound();

                if (ownerUser.Creator == null && ownerUser.Company == null)
                    return new CoWorkerError.UserIsNotACreatorOrCompany();

                return await context.CoWorkerRepository.GetTeamByOwnerId(ownerId);
            });
        }
    }
}