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
            // Se o TransactionManager fizer o Save ao fim, não precisas do SaveChanges aqui
            return true; 
        }

        public async Task<bool> ExistsByUserId(int userId)
        {
            return await _context.Creators.AnyAsync(c => c.Id == userId);
        }

        public async Task<Creator?> GetByUserId(int userId)
        {
            return await _context.Creators
                //.Include(c => c.User) // Caso precisemos dos dados do User, pensar ainda 
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
    
    }
}
