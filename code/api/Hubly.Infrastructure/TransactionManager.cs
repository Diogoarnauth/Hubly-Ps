using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Audit;
using Microsoft.Extensions.Logging;

namespace Hubly.api.Infrastructure;
public class TransactionManager : ITransactionManager
{
    private readonly HublyDbContext _context;
    private readonly AuditQueue _auditQueue;
    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(HublyDbContext context, AuditQueue auditQueue, ILogger<TransactionManager> logger)
    {
        _context = context;
        _auditQueue = auditQueue;
        _logger = logger;
    }

    public async Task<TResult> Run<TResult>(Func<ITransactionContext, Task<TResult>> operation)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transactionContext = new TransactionContext(_context);
            var result = await operation(transactionContext);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}