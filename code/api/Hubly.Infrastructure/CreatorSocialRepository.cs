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
                .Include(p => p.Sectors)
                .Include(csp => csp.Platform)
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
        public async Task<CreatorSocialProfile?> EditCreatorSocialProfile(int userId, int socialProfileId, string user_name, string link, string description, int followers_count, decimal? priceMin, decimal? priceMax, List<Sector> sectors)
        {
            var creatorSocialProfile = await _context.CreatorSocialProfiles
                .Include(c => c.Sectors)
                .FirstOrDefaultAsync(c => c.Id == socialProfileId && c.CreatorId == userId);

            if (creatorSocialProfile == null) return null;

            creatorSocialProfile.PlatformUserName = user_name;
            creatorSocialProfile.Link = link;
            creatorSocialProfile.Description = description;
            creatorSocialProfile.FollowersCount = followers_count;
            creatorSocialProfile.PriceMin = priceMin;
            creatorSocialProfile.PriceMax = priceMax;

            creatorSocialProfile.Sectors.Clear();
            if (sectors != null && sectors.Any())
            {
                foreach (var sector in sectors)
                {
                    _context.Set<Sector>().Attach(sector);
                    creatorSocialProfile.Sectors.Add(sector);
                }
            }

            await _context.SaveChangesAsync();
            return creatorSocialProfile;
        }

        public async Task<PagedResponse<CreatorSocialProfile>> Search(int? platform_id, string? platform_user_name, int? followers_count_min, int? followers_count_max, decimal? price_min, decimal? price_max, List<string>? sectors, int page, int page_size)
        {
            var query = _context.CreatorSocialProfiles
                .Include(p => p.Sectors)
                .AsNoTracking() 
                .AsQueryable();

            if (platform_id.HasValue) query = query.Where(p => p.PlatformId == platform_id.Value);

            if (!string.IsNullOrWhiteSpace(platform_user_name))
            {
                query = query.Where(p => EF.Functions.ILike(p.PlatformUserName, $"%{platform_user_name}%"));
            }

            if (followers_count_min.HasValue) query = query.Where(p => p.FollowersCount >= followers_count_min.Value);
            
            if (followers_count_max.HasValue) query = query.Where(p => p.FollowersCount <= followers_count_max.Value);

            if (price_min.HasValue) query = query.Where(p => p.PriceMin >= price_min.Value);

            if (price_max.HasValue) query = query.Where(p => p.PriceMax <= price_max.Value);

            if (sectors != null && sectors.Any())
            {
                var sectorsLower = sectors.Select(s => s.ToLower()).ToList();
                query = query.Where(p => p.Sectors.Any(s => sectorsLower.Contains(s.SectorName.ToLower())));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.FollowersCount) 
                .Skip((page - 1) * page_size)
                .Take(page_size)
                .ToListAsync();

            return new PagedResponse<CreatorSocialProfile>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = page_size
            };
        }
        
    }
}
