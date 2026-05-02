using Microsoft.EntityFrameworkCore;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Infrastructure.Data;


namespace Hubly.api.Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
        private readonly HublyDbContext _context;

        public MessageRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message.Id;
        }

        public async Task<Message?> GetById(int messageId)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<Message>> GetMessages(int conversationId, int page, int pageSize)
        {
            var query = _context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .AsQueryable();

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Message>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<Message?> GetLastMessageByConversation(int conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }
    }
}