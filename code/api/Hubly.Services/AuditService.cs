namespace Hubly.api.Services;

using Hubly.api.Domain.Entities;    
using Hubly.api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Hubly.api.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Hubly.api.Services.Problems;
using OneOf;


public class AuditService : IAuditService
{
    private readonly AuditFormatter _formatter;
    private readonly IAuditRepository _auditRepository; // Ou usa o context aqui
    private readonly ITransactionManager _transactionManager;


    public AuditService(ITransactionManager transactionManager, AuditFormatter formatter, IAuditRepository auditRepository)
    {
        _formatter = formatter;
        _auditRepository = auditRepository;
        _transactionManager = transactionManager;

    }

    // Nota: Recebe um objeto "data" (payload) com as variáveis necessárias
    public async Task LogAction(string actionName, int? userId, int? coWorkerId, object payload)
    {
        
        Console.WriteLine($"mensagem antes de esta a ser registrada:  {actionName}, {userId}, coWorker:{coWorkerId} ${payload}" );

    
        var message = _formatter.Format(actionName, payload, userId, coWorkerId);

        Console.WriteLine("mensagem que esta a ser registrada: " + message);
        
        var log = new AuditLog {
            Action = message,
            UserId = userId,
            CoWorkerId = coWorkerId,
            Timestamp = DateTime.UtcNow
        };

        await _auditRepository.AddLog(log);
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