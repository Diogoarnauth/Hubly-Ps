namespace Hubly.api.Infrastructure.Interfaces;

public interface ITransactionManager
{
    Task<TResult> Run<TResult>(Func<ITransactionContext, Task<TResult>> operarion);
}