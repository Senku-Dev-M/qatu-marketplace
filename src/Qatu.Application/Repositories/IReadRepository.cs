using System.Linq.Expressions;

namespace Qatu.Application;

public interface IReadRepository
{
    Task<T?> FindAsync<T>(Guid id) where T : class;
    Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
}
