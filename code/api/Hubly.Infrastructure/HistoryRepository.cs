using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly HublyDbContext _context;

        public HistoryRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddView(ProfileViewHistory history)
        {
            await _context.Set<ProfileViewHistory>().AddAsync(history);
            return true;
        }

        public async Task<List<Creator>> GetTopTrendingCreators(int limit)
        {
            var topIds = await _context.ProfileViewHistory
                .Where(h => h.ViewedCreatorId != null)
                .GroupBy(h => h.ViewedCreatorId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key.Value)
                .ToListAsync();

            var creators = await _context.Creators
                .Include(c => c.SocialProfiles)
                    .ThenInclude(sp => sp.Platform)
                .Where(c => topIds.Contains(c.Id))
                .ToListAsync();

            return creators
                .OrderBy(c => topIds.IndexOf(c.Id))
                .ToList();
        }

        public async Task<List<Company>> GetTopTrendingCompanies(int limit)
        {
            var topIds = await _context.ProfileViewHistory
                .Where(h => h.ViewedCompanyId != null)
                .GroupBy(h => h.ViewedCompanyId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key.Value)
                .ToListAsync();

            var companies = await _context.Companies
                .Include(c => c.Sectors)
                .Where(c => topIds.Contains(c.Id))
                .ToListAsync();

            return companies
                .OrderBy(c => topIds.IndexOf(c.Id))
                .ToList();
        }

        public async Task<List<ProfileViewHistory>> GetUserHistory(int userId, int limit = 20)
        {
            return await _context.ProfileViewHistory
                .Include(h => h.ViewedCompany)
                .Include(h => h.ViewedCreator)
                .Where(h => h.ViewerUserId == userId)
                .OrderByDescending(h => h.ViewedAt)
                .Take(limit)
                .AsNoTracking() //TODO() verificar se temos de ter isto 
                .ToListAsync();
        }
    }
}