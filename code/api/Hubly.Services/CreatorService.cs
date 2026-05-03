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

                if (await context.CompanyRepository.ExistsByUserId(userId)) return new CreatorError.UserAlreadyRegisteredAsCompany();

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
            return await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserIdSocialProfiles(targetCreatorId);
                if (creator == null) return new CreatorError.CreatorNotFound();

                return creator;
            });
        }

        public async Task<OneOf<bool, CreatorError>> RateCreator(int evaluatorId, int creatorId, int rating)
        {
            if (!_creatorsDomain.IsValidRating(rating))
                return new CreatorError.InvalidRating();

            return await _transactionManager.Run<OneOf<bool, CreatorError>>(async (context) =>
            {
                var creator = await context.CreatorRepository.GetByUserId(creatorId);
                if (creator == null)
                {
                    return new CreatorError.CreatorNotFound();
                }

                var alreadyRated = await context.CreatorRepository.HasUserRatedCreator(evaluatorId, creatorId);
                if (alreadyRated)
                {
                    return new CreatorError.ErrorRatingCreator();
                }

                var newRatingEntry = new CreatorRating
                {
                    EvaluatorId = evaluatorId,
                    TargetCreatorId = creatorId,
                    RatingValue = rating,
                    RatedAt = DateTime.UtcNow
                };

                await context.CreatorRepository.AddRating(newRatingEntry);

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


        public async Task<OneOf<int?, CreatorError>> GetUserRatingForCreator(int currentUserId, int creatorId)
        {
            return await _transactionManager.Run<OneOf<int?, CreatorError>>(async (context) =>
            {
                if (!await context.CreatorRepository.ExistsByUserId(creatorId))
                {
                    return new CreatorError.CreatorNotFound();
                }

                var ratingEntry = await context.CreatorRepository.GetUserRating(currentUserId, creatorId);

                return ratingEntry?.RatingValue;
            });
        }


        public async Task<OneOf<(CreatorSocialProfile Profile, bool IsOwner), CreatorError>> GetSocialProfileById(int creatorProfileId, int userId)
        {
            return await _transactionManager.Run<OneOf<(CreatorSocialProfile, bool), CreatorError>>(async (context) =>
            {
                var profile = await context.CreatorSocialRepository.GetById(creatorProfileId);
                if (profile == null) return new CreatorError.SocialProfileNotFound();

                bool isOwner = profile.CreatorId == userId;

                if (!isOwner)
                {
                    try
                    {
                        var historyEntry = new ProfileViewHistory
                        {
                            ViewerUserId = userId,
                            ViewedSocialProfileId = creatorProfileId,
                            ViewedAt = DateTime.UtcNow
                        };

                        await context.HistoryRepository.AddView(historyEntry);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao gravar histórico: {ex.Message}");
                    }
                }

                return (profile, isOwner);
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

        public async Task<OneOf<List<Sector>, CreatorError>> GetAllSectors()
        {
            return await _transactionManager.Run<OneOf<List<Sector>, CreatorError>>(async (context) =>
            {
                var sectors = await context.CreatorRepository.GetAllSectors();

                if (sectors == null) return new CreatorError.SectorsNotFound();

                return sectors;
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

                //if (await context.CreatorSocialRepository.ExistsByPlatformAndUsername(creatorSocialProfile.PlatformId, user_name)) return new CreatorError.SocialProfileAlreadyExists();

                var foundSectors = await context.CreatorRepository.GetSectorByName(sectors);
                if (foundSectors.Count != sectors.Count) return new CreatorError.InvalidSectorName();

                var updatedCreatorSocialProfile = await context.CreatorSocialRepository.EditCreatorSocialProfile(
                    userId, socialProfileId, user_name, link, description, followers_count, priceMin, priceMax, foundSectors);

                if (updatedCreatorSocialProfile == null) return new CreatorError.FailedToGetCreatorSocialProfileInfo();

                return updatedCreatorSocialProfile;
            });

            return result;
        }

        public async Task<OneOf<PagedResponse<CreatorSocialProfile>, CreatorError>> Search(int? platform_id, string? platform_user_name, int? followers_count_min, int? followers_count_max, decimal? price_min, decimal? price_max, List<string>? sectors, int page, int page_size)
        {
            page = page <= 0 ? 1 : page;
            page_size = page_size <= 0 ? 10 : (page_size > 100 ? 100 : page_size);

            return await _transactionManager.Run<OneOf<PagedResponse<CreatorSocialProfile>, CreatorError>>(async (context) =>
            {
                var results = await context.CreatorSocialRepository.Search(
                    platform_id,
                    platform_user_name,
                    followers_count_min,
                    followers_count_max,
                    price_min,
                    price_max,
                    sectors,
                    page,
                    page_size
                );

                if (results == null) return new CreatorError.FailedToGetCreatorInfo();

                return results;
            });
        }



        public async Task<OneOf<List<CreatorSocialProfile>, CreatorError>> GetTrendingCreators(int limit)
        {
            return await _transactionManager.Run<OneOf<List<CreatorSocialProfile>, CreatorError>>(async (context) =>
            {
                var trendingProfiles = await context.HistoryRepository.GetTopTrendingCreators(limit);

                if (trendingProfiles == null)
                    return new List<CreatorSocialProfile>();

                return trendingProfiles;
            });
        }

        public async Task<OneOf<Creator, CreatorError>> Edit(int user_id, string artisticName)
        {
            var result = await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
            {
                if (await context.CompanyRepository.ExistsByUserId(user_id)) return new CreatorError.UserAlreadyRegisteredAsCompany();

                var creatorExists = await context.CreatorRepository.GetByUserId(user_id);
                if (creatorExists == null) return new CreatorError.FailedToGetCreatorInfo();

                var updatedCreator = await context.CreatorRepository.Edit(user_id, artisticName);

                if (updatedCreator == null) return new CreatorError.FailedToGetCreatorInfo();

                return updatedCreator;
            });

            return result;
        }

        public async Task<OneOf<List<CreatorSocialProfile>, CreatorError>> GetRecommendedCreators(int userId)
        {
            return await _transactionManager.Run<OneOf<List<CreatorSocialProfile>, CreatorError>>(async (context) =>
            {
                var interests = await context.HistoryRepository.GetCreatorInterests(userId);

                bool hasHistory = interests.SectorFrequencies.Any() ||
                                  interests.PlatformFrequencies.Any();

                if (!hasHistory)
                {
                    Console.WriteLine("Hubly: Usuário sem histórico. Retornando perfis em alta.");

                    var trending = await context.HistoryRepository.GetTopTrendingCreators(10);

                    return trending ?? new List<CreatorSocialProfile>();
                }

                var recommendations = await context.CreatorRepository.GetRecommendedSocialProfilesByScore(userId, interests);

                return recommendations ?? new List<CreatorSocialProfile>();
            });
        }
    }
}