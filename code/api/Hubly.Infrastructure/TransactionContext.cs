using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Infrastructure.Data;

namespace Hubly.api.Infrastructure;

public class TransactionContext : ITransactionContext
{
    private readonly HublyDbContext _context;

    public TransactionContext (HublyDbContext context)
    {
        _context = context;
        UserRepository = new UserRepository(context);
    }

    public IUserRepository UserRepository { get;}
}