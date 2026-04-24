using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IMessageRepository
{
    Task<int> AddMessage(Message message);
    Task<Message?> GetById(int messageId);
    Task UpdateMessage(Message message);
    Task<PagedResponse<Message>> GetMessages(int conversationId, int page, int pageSize);
}