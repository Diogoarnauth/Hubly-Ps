using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace Hubly.api.Services
{
    public class CreatorService : ICreatorService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly CreatorsDomain _creatorsDomain;

        public CreatorService(
            ITransactionManager transactionManager,
            IConfiguration configuration,
            CreatorsDomain creatorsDomain
        )
        {
            _transactionManager = transactionManager;
            _creatorsDomain = creatorsDomain;
        }

        public async Task<OneOf<Creator, CreatorError>> Register(int userId, string artisticName)
        {
            // Validação de Domínio única agora
            Console.WriteLine("antes no if");

            if (!_creatorsDomain.IsValidArtisticName(artisticName))
            {
                Console.WriteLine("Entrei no if");
                return new CreatorError.InvalidArtisticName();
            }
            Console.WriteLine("SAIII no if");

            return await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                // Verificações cruzadas
                if (await context.CreatorRepository.ExistsByUserId(userId))
                    return new CreatorError.CreatorAlreadyExists();
                Console.WriteLine("tou no sitio errado ");

                if (await context.CompanyRepository.ExistsByUserId(userId))
                    return new CreatorError.UserAlreadyRegisteredAsCompany();

                var newCreator = new Creator
                {
                    Id = userId,
                    ArtisticName = artisticName,
                    // Os outros campos (rating, counts) são inicializados 
                    // pelos defaults da BD ou pelos valores padrão da classe.
                    IsVerified = false,
                    AvailabilityStatus = "AVAILABLE",
                    GlobalRating = 0,
                    RatingsCount = 0,
                    ChatsStartedCount = 0,
                    ChatsRespondedCount = 0
                };

                await context.CreatorRepository.RegisterCreator(newCreator);

                return newCreator;
            });


        }

        public async Task<OneOf<Creator, CreatorError>> UpdateStatus(int userId, string newStatus)
        {
            if (!_creatorsDomain.IsValidAvailabilityStatus(newStatus)) return new CreatorError.InvalidAvailabilityStatus();

            return await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserId(userId);

                if (creator == null) return new CreatorError.CreatorNotFound();

                creator.AvailabilityStatus = newStatus.ToUpper();

                var success = await context.CreatorRepository.UpdateStatus(creator.Id, newStatus);

                if (success == null) return new CreatorError.FailedToUpdateStatus();

                return creator;
            });
        }
    }
}