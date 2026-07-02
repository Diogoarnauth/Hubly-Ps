using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces;

public interface IAuditRepository
{
    Task AddLog(AuditLog log);
    Task<PagedResponse<AuditLog>> Search(int userId, int page, int pageSize);
}