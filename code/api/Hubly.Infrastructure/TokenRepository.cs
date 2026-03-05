using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hubly.api.Infrastructure
{
    public class TokenRepository : ITokenRepository
    {
        private readonly HublyDbContext _context;

        public TokenRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<Token?> GetTokenByValidation(string tokenValidation)
        {
            return await _context.Tokens
                .FirstOrDefaultAsync(t => t.TokenValidation == tokenValidation);
        }

        public async Task<Token?> GetTokenByUserId(int userId)
        {
            return await _context.Tokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateToken(Token token)
        {
            await _context.Tokens.AddAsync(token);
            return true;
        }

        public async Task<bool> DeleteToken(int userId, string tokenValidation)
        {
            var token = await _context.Tokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.TokenValidation == tokenValidation);
            
            if (token == null) return false;

            _context.Tokens.Remove(token);
            return true;
        }

        public async Task<bool> DeleteTokenByValidation(string tokenValidation)
        {
            var token = await GetTokenByValidation(tokenValidation);
            if (token == null) return false;

            _context.Tokens.Remove(token);
            return true;
        }

        // Atualiza a data de último uso
        public async Task<bool> RefreshToken(string tokenValidation)
        {
            var token = await GetTokenByValidation(tokenValidation);
            if (token == null) return false;

            token.LastUsedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return true;
        }
    }
}