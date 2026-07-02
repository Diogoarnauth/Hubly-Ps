namespace Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using OneOf;

public interface IAuditService
{
    Task LogAction(string actionName, int? userId, int? coWorkerId, string endpoint, object payload);
    Task<OneOf<PagedResponse<AuditLog>, UserError>> GetAuditLogs(int userId, int Page, int PageSize);

}