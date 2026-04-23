using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.Domain.Entities.Chats;
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
                .Where(c => c.Participants.Any(p => p.UserId == userId1) && 
                            c.Participants.Any(p => p.UserId == userId2))
                .Where(c => c.Participants.Count == 2)
                .FirstOrDefaultAsync();
        }
    }
}