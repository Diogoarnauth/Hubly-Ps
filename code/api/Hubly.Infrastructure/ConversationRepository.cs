using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hubly.api.Infrastructure
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly HublyDbContext _context;

        public ConversationRepository(HublyDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddConversation(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return conversation.Id;
        }
        public async Task<Conversation?> GetConversationByParticipants(
            int? sCompId, int? sProfId,
            int? rCompId, int? rProfId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                // Garante que a conversa tem exatamente 2 pessoas
                .Where(c => c.Participants.Count == 2)
                .Where(c =>
                    // Verifica se um dos participantes é o Sender (Tipado)
                    c.Participants.Any(p =>
                        (sCompId != null && p.CompanyId == sCompId && p.SocialProfileId == null) ||
                        (sProfId != null && p.SocialProfileId == sProfId && p.CompanyId == null)
                    )
                    &&
                    // Verifica se o outro participante é o Receiver (Tipado)
                    c.Participants.Any(p =>
                        (rCompId != null && p.CompanyId == rCompId && p.SocialProfileId == null) ||
                        (rProfId != null && p.SocialProfileId == rProfId && p.CompanyId == null)
                    )
                )
                .FirstOrDefaultAsync();
        }
        public async Task<bool> IsUserParticipant(int conversationId, int userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
        }

        public async Task<Conversation?> GetById(int id)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Update(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Conversation>> GetCreatorConversationsByProfile(int userId, int socialProfileId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.SocialProfile)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.SocialProfileId == socialProfileId))
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<List<Conversation>> GetConversationsByCompany(int userId, int companyId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.SocialProfile)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.CompanyId == companyId))
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }


    }
}