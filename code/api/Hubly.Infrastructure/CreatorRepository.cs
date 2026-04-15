using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure
{
    public class CreatorRepository : ICreatorRepository
    {
        private readonly HublyDbContext _context;

        public CreatorRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterCreator(Creator newCreator)
        {
            await _context.Creators.AddAsync(newCreator);
            return true;
        }

        public async Task<bool> ExistsByUserId(int userId)
        {
            return await _context.Creators.AnyAsync(c => c.Id == userId);
        }

        public async Task<Creator?> GetByUserId(int userId)
        {
            return await _context.Creators
                .FirstOrDefaultAsync(c => c.Id == userId);
        }

        public async Task<Creator?> UpdateStatus(int userId, string newStatus)
        {
            var creator = await _context.Creators.FindAsync(userId);
            if (creator == null) return null;
            creator.AvailabilityStatus = newStatus;
            _context.Creators.Update(creator);
            await _context.SaveChangesAsync();
            return creator;
        }

        public async Task<bool> UpdateRating(Creator creator)
        {
            _context.Creators.Update(creator);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IncrementChatsStarted(int userId)
        {
            var creator = await _context.Creators.FindAsync(userId);
            if (creator == null) return false;

            creator.ChatsStartedCount++;

            _context.Creators.Update(creator);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IncrementChatsResponded(int userId)
        {
            var creator = await _context.Creators.FindAsync(userId);
            if (creator == null) return false;

            creator.ChatsRespondedCount++;

            _context.Creators.Update(creator);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Creator?> GetByUserIdSocialProfiles(int userId)
        {
            return await _context.Creators
            .Include(c => c.SocialProfiles)
                .ThenInclude(csp => csp.Platform)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == userId);
        }


        //About Sector
        public async Task<List<Sector>> GetSectorByName(List<string> sectorName)
        {
            return await _context.Sectors
                .Where(s => sectorName.Contains(s.SectorName))
                .ToListAsync();
        }

        public async Task<List<Sector>> GetAllSectors()
        {
            return await _context.Sectors.ToListAsync();
        }

        public async Task<Creator?> Edit(int user_id, string artisticName)
        {
            var creator = await _context.Creators.FirstOrDefaultAsync(c => c.Id == user_id);
            if (creator == null) return null;
            creator.ArtisticName = artisticName;
            await _context.SaveChangesAsync();
            return creator;
            
        }

    }
}
