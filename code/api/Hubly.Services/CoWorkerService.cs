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

        public async Task<OneOf<Success, CoWorkerError>> SendInvite(int ownerId, string email)
        {
            return await _transactionManager.Run<OneOf<Success, CoWorkerError>>(async (context) =>
            {
                var targetUser = await context.UserRepository.GetUserByEmail(email);
                if (targetUser == null) return new CoWorkerError.UserNotFound();

                if(targetUser.Id == ownerId) return new CoWorkerError.CannotInviteSelf();

                var fullTargetUser = await context.UserRepository.GetUserById(targetUser.Id);

                if (fullTargetUser == null) return new CoWorkerError.UserNotFound();
                if (fullTargetUser.Creator != null || fullTargetUser.Company != null)
                    {
                        return new CoWorkerError.UserCannotBeACoWorker();
                    }

                var existingCoWorker = await context.CoWorkerRepository.GetCoWorker(fullTargetUser.Id); //
                if(existingCoWorker != null) return new CoWorkerError.UserAlreadyACoWorker();

                // Verifica se já existe um convite pendente
                if (await context.CoWorkerRepository.InviteExists(ownerId, email)) //
                {
                    return new CoWorkerError.AlreadyInvited();
                }

                await context.CoWorkerRepository.CreateInvite(ownerId, email); //

                // Opcional: _emailService.SendCoWorkerInviteEmail(email);
                
                return new Success();
            });
        }


        public async Task<OneOf<Success, CoWorkerError>> AcceptInvite(int userId, int inviteId) //verificar validade do invite TIMESTAMP
        {
            return await _transactionManager.Run<OneOf<Success, CoWorkerError>>(async (context) =>
            {
                var invite = await context.CoWorkerInviteRepository.GetInviteById(inviteId);
                
                if (invite == null || invite.CoWorkerEmail != (await context.UserRepository.GetUserById(userId))?.Email)
                {
                    return new CoWorkerError.InviteNotFound();
                }

                if (invite.Status != "WAITING") return new CoWorkerError.Unauthorized();

                // Cria a associação efetiva
                await context.CoWorkerRepository.CreateCoWorker(userId, invite.OwnerId);
                
                // Atualiza o convite
                await context.CoWorkerInviteRepository.UpdateStatus(inviteId, "ACCEPTED");

                return new Success();
            });
        }

        public async Task<OneOf<Success, CoWorkerError>> RejectInvite(int userId, int inviteId) //verificar validade do invite TIMESTAMP
        {
            return await _transactionManager.Run<OneOf<Success, CoWorkerError>>(async (context) =>
            {
                var invite = await context.CoWorkerInviteRepository.GetInviteById(inviteId);
                
                // Apenas quem recebeu o convite pode rejeitar
                if (invite == null || invite.CoWorkerEmail != (await context.UserRepository.GetUserById(userId))?.Email)
                {
                    return new CoWorkerError.InviteNotFound();
                }

                if (invite.Status != "WAITING") return new CoWorkerError.Unauthorized();


                await context.CoWorkerInviteRepository.UpdateStatus(inviteId, "REJECTED");
                return new Success();
            });
        }

        public async Task<OneOf<List<CoWorkerInvite>, Error>> GetReceivedInvites(int userId)
        {
            return await _transactionManager.Run<OneOf<List<CoWorkerInvite>, Error>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                return await context.CoWorkerInviteRepository.GetInvitesByEmail(user.Email);
            });
        }

        public async Task<OneOf<List<CoWorkerInvite>, Error>> GetSentInvites(int userId)
        {
            return await _transactionManager.Run<OneOf<List<CoWorkerInvite>, Error>>(async (context) =>
            {
                return await context.CoWorkerInviteRepository.GetInvitesByOwner(userId);
            });
        }
    }
}