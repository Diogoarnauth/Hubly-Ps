using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace  Hubly.api.Infrastructure
{
    public class EmailConfirmationRepository : IEmailConfirmationRepository
    {
        private readonly HublyDbContext _context;


        public EmailConfirmationRepository(HublyDbContext context)
        {
            _context = context;
        }


        public async Task<EmailConfirmation> CreateConfirmationCodeAsync(int userId, string confirmationCode, int expiryHours)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var emailConfirmationCode = new EmailConfirmation
            {
                UserId = userId,
                ConfirmationCode = confirmationCode,
                CreatedAt = now,
                ExpiresAt = now + expiryHours * 3600,
                Used = false
            };
            await _context.EmailConfirmations.AddAsync(emailConfirmationCode);
            await _context.SaveChangesAsync();
            return emailConfirmationCode;
        }


        public async Task<EmailConfirmation> GetConfirmationCodeAsync(string confirmationCode)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return await _context.EmailConfirmations
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.ConfirmationCode == confirmationCode && !t.Used && t.ExpiresAt > now);
        }


        public async Task<bool> MarkConfirmationCodeAsUsedAsync(int confirmationCodeId)
        {
            var confirmationCode = await _context.EmailConfirmations.FindAsync(confirmationCodeId);
            if (confirmationCode == null)
                return false;


            confirmationCode.Used = true;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> ConfirmUserEmailAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;


            user.IsEmailConfirmed = true;
            user.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CodeExists(int userId)
        {
            return await _context.EmailConfirmations.AnyAsync(t => t.UserId == userId);
        }

        public async Task<List<EmailConfirmation>> GetExpiredConfirmationCodes()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return await _context.EmailConfirmations
                .Where(e => e.ExpiresAt < now && !e.Used)
                .ToListAsync();
        }

        public async Task DeleteConfirmationCodes(List<EmailConfirmation> confirmationCodes)
        {
            if (confirmationCodes == null || !confirmationCodes.Any())
            {
                return;
            }

            _context.EmailConfirmations.RemoveRange(confirmationCodes);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConfirmationCodesById(List<int> codeIds)
        {
            if (codeIds == null || !codeIds.Any())
            {
                return;
            }

            var codesToDelete = await _context.EmailConfirmations
                .Where(ec => codeIds.Contains(ec.Id))
                .ToListAsync();

            if (codesToDelete.Any())
            {
                _context.EmailConfirmations.RemoveRange(codesToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
