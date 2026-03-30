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

                Console.WriteLine("creatorId:", creator);

                if (creator == null) return new CreatorError.CreatorNotFound();

                creator.AvailabilityStatus = newStatus.ToUpper();

                var success = await context.CreatorRepository.UpdateStatus(creator.Id, newStatus);

                if (success == null) return new CreatorError.FailedToUpdateStatus();

                return creator;
            });
        }


       /* public async Task<OneOf<Creator, CompanyError>> GetById(int userId)
        {
            var result = await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                var company = await context.CreatorRepository.GetByUserId(userId);
                if (company == null) return new CreatorError.CreatorNotFound();

                return Creator;
            });

            return result;
        }
*/
        public async Task<OneOf<bool, CreatorError>> RateCreator(int creatorId, int rating)
        {
            if (!_creatorsDomain.IsValidRating(rating)) return new CreatorError.InvalidRating();


            return await _transactionManager.Run<OneOf<bool, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserId(creatorId);

                if (creator == null)
                {
                    return new CreatorError.CreatorNotFound();
                }

                var (newGlobalRating, newRatingsCount) = _creatorsDomain.CalculateNewRating(
                    creator.GlobalRating,
                    creator.RatingsCount,
                    rating
                );

                creator.GlobalRating = newGlobalRating;
                creator.RatingsCount = newRatingsCount;

                var success = await context.CreatorRepository.UpdateRating(creator);

                if (!success)
                {
                    return new CreatorError.ErrorRatingCreator();
                }

                return true;
            });
        }

        public async Task<OneOf<CreatorSocialProfile, CreatorError>> AddSocialProfile(int userId, string user_name, string link, int followers_count, decimal? priceMin, decimal? priceMax, int platform_id)
        {
            if (!_creatorsDomain.IsValidPriceRange(priceMin, priceMax))
            {
                return new CreatorError.InvalidPriceRange();
            }
            return await _transactionManager.Run<OneOf<CreatorSocialProfile, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserId(userId);
                if (creator == null)
                {
                    return new CreatorError.CreatorNotFound();
                }

                if (!await context.SocialPlatformRepository.Exists(platform_id))
                {
                    return new CreatorError.PlatformNotFound();
                }

                if (await context.CreatorSocialRepository.HasProfileInPlatform(userId, platform_id))
                {
                    return new CreatorError.SocialProfileAlreadyExists();
                }

                var newProfile = new CreatorSocialProfile
                {
                    CreatorId = userId,
                    PlatformId = platform_id,
                    PlatformUserName = user_name,
                    Link = link,
                    FollowersCount = followers_count,
                    PriceMin = priceMin,
                    PriceMax = priceMax
                };

                await context.CreatorSocialRepository.Add(newProfile);

                return newProfile;
            });

        }

        public async Task<OneOf<bool, CreatorError>> RemoveSocialProfile(int userId, int profileId)
        {
            return await _transactionManager.Run<OneOf<bool, CreatorError>>(async (context) =>
            {
                var profile = await context.CreatorSocialRepository.GetById(profileId);

                if (profile == null)
                {
                    return new CreatorError.SocialProfileNotFound();
                }

                if (profile.CreatorId != userId)
                {
                    return new CreatorError.SocialProfileNotFound();
                }

                context.CreatorSocialRepository.Delete(profile);

                return true;
            });
        }

    }

}