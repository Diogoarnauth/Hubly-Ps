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

            if (!_creatorsDomain.IsValidArtisticName(artisticName)) return new CreatorError.InvalidArtisticName();
            
            return await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                if (await context.CreatorRepository.ExistsByUserId(userId)) return new CreatorError.CreatorAlreadyExists();

                if (await context.CompanyRepository.ExistsByUserId(userId))  return new CreatorError.UserAlreadyRegisteredAsCompany();

                var newCreator = new Creator
                {
                    Id = userId,
                    ArtisticName = artisticName,
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


        public async Task<OneOf<Creator, CreatorError>> GetById(int targetCreatorId, int viewerId)
        {
            var result = await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserIdSocialProfiles(targetCreatorId);
                if (creator == null) return new CreatorError.CreatorNotFound();
                
                try
                    {
                        var historyEntry = new ProfileViewHistory
                        {
                            ViewerUserId = viewerId,
                            ViewedCreatorId = targetCreatorId,
                            ViewedAt = DateTime.UtcNow
                        };

                        await context.HistoryRepository.AddView(historyEntry);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao gravar histórico: {ex.Message}");
                    }
                
                return creator;
            });

            return result;
        }

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


        public async Task<OneOf<CreatorSocialProfile, CreatorError>> GetSocialProfileById(int creatorProfileId, int userId)
        {
            return await _transactionManager.Run<OneOf<CreatorSocialProfile, CreatorError>>(async (context) =>
            {
                var profile = await context.CreatorSocialRepository.GetById(creatorProfileId);

                if (profile == null) return new CreatorError.SocialProfileNotFound();

                return profile;
            });
        }

        public async Task<OneOf<CreatorSocialProfile, CreatorError>> AddSocialProfile(int userId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax, int platform_id, List<String> sectors)
        {
            if (!_creatorsDomain.IsValidPriceRange(priceMin, priceMax)) return new CreatorError.InvalidPriceRange();
            if (!_creatorsDomain.IsValidPriceRange(priceMin, priceMax)) return new CreatorError.InvalidPriceRange();
            if (!_creatorsDomain.IsValidArtisticName(user_name)) return new CreatorError.InvalidArtisticName();
            if (!_creatorsDomain.IsValidSocialLink(link)) return new CreatorError.InvalidWebSiteLink();
            if (!_creatorsDomain.IsValidFollowersCount(followers_count)) return new CreatorError.InvalidFollowersCount();

            return await _transactionManager.Run<OneOf<CreatorSocialProfile, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserId(userId);
                if (creator == null) return new CreatorError.CreatorNotFound();

                if (!await context.SocialPlatformRepository.Exists(platform_id))
                {
                    return new CreatorError.PlatformNotFound();
                }

                if (await context.CreatorSocialRepository.ExistsByPlatformAndUsername(platform_id, user_name))
                {
                    return new CreatorError.SocialProfileAlreadyExists();
                }

                var foundSectors = await context.CreatorRepository.GetSectorByName(sectors);

                if (foundSectors.Count != sectors.Count) return new CreatorError.InvalidSectorName();


                var newProfile = new CreatorSocialProfile
                {
                    CreatorId = userId,
                    PlatformId = platform_id,
                    PlatformUserName = user_name,
                    Link = link,
                    Description = description,
                    FollowersCount = followers_count,
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Sectors = foundSectors
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

        public async Task<OneOf<CreatorSocialProfile, CreatorError>> EditCreatorSocialProfile(int userId, int socialProfileId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax, List<String> sectors)
        {
            if (!_creatorsDomain.IsValidPriceRange(priceMin, priceMax)) return new CreatorError.InvalidPriceRange();
            if (!_creatorsDomain.IsValidArtisticName(user_name)) return new CreatorError.InvalidArtisticName();
            if (!_creatorsDomain.IsValidSocialLink(link)) return new CreatorError.InvalidWebSiteLink();
            if (!_creatorsDomain.IsValidFollowersCount(followers_count)) return new CreatorError.InvalidFollowersCount();

            var result = await _transactionManager.Run<OneOf<CreatorSocialProfile, CreatorError>>(async (context) =>
            {
                if (await context.CompanyRepository.ExistsByUserId(userId)) return new CreatorError.UserAlreadyRegisteredAsCompany();

                var creatorSocialProfile = await context.CreatorSocialRepository.GetById(socialProfileId);
                if (creatorSocialProfile == null) return new CreatorError.SocialProfileNotFound();

                if (creatorSocialProfile.CreatorId != userId) return new CreatorError.ProfileDoesntBellongToYou();

                if (await context.CreatorSocialRepository.ExistsByPlatformAndUsername(creatorSocialProfile.PlatformId, user_name)) return new CreatorError.SocialProfileAlreadyExists();

                var foundSectors = await context.CreatorRepository.GetSectorByName(sectors);
                if (foundSectors.Count != sectors.Count) return new CreatorError.InvalidSectorName();

                var updatedCreatorSocialProfile = await context.CreatorSocialRepository.EditCreatorSocialProfile(
                    userId, socialProfileId, user_name, link, description, followers_count, priceMin, priceMax, foundSectors);

                if (updatedCreatorSocialProfile == null) return new CreatorError.FailedToGetCreatorSocialProfileInfo();

                return updatedCreatorSocialProfile;
            });

            return result;
        }

    }

}