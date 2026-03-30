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

    }
}
