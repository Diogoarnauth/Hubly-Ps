using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly HublyDbContext _context;

        public UserRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUser(User newUser)
        {
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync(); //TODO() Ver depois pq pode gerar conflito com o safe da transaction
            return true;
        }

        public async Task<bool> UserExistsWithEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        
       public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task EditUser(int userId, string newUsername)
        {
            var user = await _context.Users.FindAsync(userId);
            user.Name = newUsername;
            _context.Users.Update(user);
        }

}
}
