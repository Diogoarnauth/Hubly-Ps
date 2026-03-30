using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure
{
    public class SocialPlatformRepository : ISocialPlatformRepository
    {
        private readonly HublyDbContext _context;

        public SocialPlatformRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Exists(int platformId)
        {
            return await _context.SocialPlatforms.AnyAsync(p => p.Id == platformId);
        }
       
    
    }
}
