using System.Linq.Expressions;
using Qatu.Domain;

namespace Qatu.Application;

public interface IAppRepository
{
    Task<T?> FindAsync<T>(Guid id) where T : class;
    Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    void Add<T>(T entity) where T : class;
    Task SaveAsync();
    Task<TResult> InTransactionAsync<TResult>(Func<Task<TResult>> operation);
    Task<bool> TryDecrementStockAsync(Guid productId, int quantity);
}
