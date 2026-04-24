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

        public async Task<Conversation?> GetConversationBetweenUsers(int userId1, int userId2)
        {
            return await _context.Conversations
                .Include(c => c.Participants) 
                .Where(c => c.Participants.Any(p => p.UserId == userId1) && 
                            c.Participants.Any(p => p.UserId == userId2))
                .Where(c => c.Participants.Count == 2)
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
    }
}