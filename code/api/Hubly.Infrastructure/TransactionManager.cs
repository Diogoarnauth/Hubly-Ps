using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure;
public class TransactionManager : ITransactionManager
{
    private readonly ApplicationDbContext _context;

    public TransactionManager(ApplicationDbContext context)
    {
        _context = context;
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
