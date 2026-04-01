using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure
{
    public class CreatorSocialRepository : ICreatorSocialRepository
    {
        private readonly HublyDbContext _context;

        public CreatorSocialRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task Add(CreatorSocialProfile profile)
        {
            await _context.CreatorSocialProfiles.AddAsync(profile);
        }

        public async Task<bool> HasProfileInPlatform(int userId, int platformId)
        {
            return await _context.CreatorSocialProfiles.AnyAsync(p => p.CreatorId == userId && p.PlatformId == platformId);
        }

        public async Task<CreatorSocialProfile?> GetById(int profileId)
        {
            return await _context.CreatorSocialProfiles
                .FirstOrDefaultAsync(p => p.Id == profileId);
        }

        public void Delete(CreatorSocialProfile profile)
        {
            _context.CreatorSocialProfiles.Remove(profile);
        }

        public async Task<bool> ExistsByPlatformAndUsername(int platformId, string username)
        {
            return await _context.CreatorSocialProfiles
                .AnyAsync(p => p.PlatformId == platformId && p.PlatformUserName == username);
        }
        public async Task<CreatorSocialProfile?> EditCreatorSocialProfile(int userId, int socialProfileId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax)
        {
            var creatorSocialProfile = await _context.CreatorSocialProfiles.FindAsync(socialProfileId);
            if (creatorSocialProfile == null) return null;

            creatorSocialProfile.PlatformUserName = user_name;
            creatorSocialProfile.Link = link;
            creatorSocialProfile.Description = description;
            creatorSocialProfile.FollowersCount = followers_count;
            creatorSocialProfile.PriceMin = priceMin;
            creatorSocialProfile.PriceMax = priceMax;

            _context.CreatorSocialProfiles.Update(creatorSocialProfile);
            await _context.SaveChangesAsync();

            _context.Entry(creatorSocialProfile).State = EntityState.Detached;

            return creatorSocialProfile;
        }

    }
}
