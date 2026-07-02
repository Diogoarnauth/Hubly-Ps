using Hubly.api.Domain.Entities; 
using Hubly.api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Hubly.api.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Hubly.api.Services.Problems;
using System.Text.Json; 
using OneOf;

namespace Hubly.api.Services;
public class AuditService : IAuditService
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AuditService> _logger;

    public AuditService(ITransactionManager transactionManager, ILogger<AuditService> logger)
    {
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task LogAction(string actionName, int? userId, int? coWorkerId, string endpoint, object payload)
    {
        try
        {
            // Usamos o TransactionManager para garantir a persistência
            await _transactionManager.Run<Task>(async (context) =>
            {
                string payloadJson = JsonSerializer.Serialize(payload);
                var log = new AuditLog
                {
                    Action = actionName,
                    UserId = userId,
                    CoWorkerId = coWorkerId,
                    Timestamp = DateTime.UtcNow,
                    Endpoint = endpoint,
                    Payload = payloadJson
                };

                context.AuditRepository.AddLog(log);
                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gravar log via TransactionManager para a ação: {Action}", actionName);
        }
    }

    
        public async Task<OneOf<PagedResponse<AuditLog>, UserError>> GetAuditLogs(int userId, int Page, int PageSize)
        {
            Page = Page <= 0 ? 1 : Page;
            PageSize = PageSize <= 0 ? 10 : (PageSize > 100 ? 100 : PageSize);

            return await _transactionManager.Run<OneOf<PagedResponse<AuditLog>, UserError>>(async (context) =>
            {
                var results = await context.AuditRepository.Search(
                    userId,
                    Page,
                    PageSize
                );

                if (results == null) return new UserError.FailedToGetLogs();

                return results;
            });
        }
}