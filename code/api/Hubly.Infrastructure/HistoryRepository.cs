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