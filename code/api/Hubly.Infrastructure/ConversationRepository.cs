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

        public async Task<Conversation?> GetConversationWithParticipants(int id)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
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

        public async Task<List<Conversation>> GetConversationsByCompany(int userId,int companyId)
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

        public async Task UpdateLastReadMessage(int conversationId, int userId, int messageId)
        {
            var readStatus = await _context.MessageReadStatuses
                .FirstOrDefaultAsync(rs => rs.ConversationId == conversationId && rs.UserId == userId);

            if (readStatus == null)
            {
                readStatus = new MessageReadStatus
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    LastReadMessageId = messageId,
                    LastReadAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
                _context.MessageReadStatuses.Add(readStatus);
            }
            else
            {
                readStatus.LastReadMessageId = messageId;
                readStatus.LastReadAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                _context.MessageReadStatuses.Update(readStatus);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetLastReadMessageId(int conversationId, int userId)
        {
            var readStatus = await _context.MessageReadStatuses
                .FirstOrDefaultAsync(rs => rs.ConversationId == conversationId && rs.UserId == userId);

            return readStatus?.LastReadMessageId;
        }

        public async Task<int> GetUnreadMessageCount(int conversationId, int userId)
        {
            var lastReadId = await GetLastReadMessageId(conversationId, userId);

            if (!lastReadId.HasValue)
            {
                // If never read, count all non-deleted messages
                return await _context.Messages
                    .CountAsync(m => m.ConversationId == conversationId && !m.IsDeleted);
            }

            return await _context.Messages
                .CountAsync(m => m.ConversationId == conversationId && m.Id > lastReadId.Value && !m.IsDeleted);
        }
    }
}