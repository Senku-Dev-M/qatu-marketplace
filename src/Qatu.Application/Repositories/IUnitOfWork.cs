namespace Qatu.Application;

public interface IUnitOfWork
{
    Task SaveAsync();
    Task<TResult> InTransactionAsync<TResult>(Func<Task<TResult>> operation);
}
