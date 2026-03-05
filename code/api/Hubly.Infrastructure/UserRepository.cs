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

        }
}
