using System.Linq.Expressions;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Tests.Support;

internal sealed class MemoryStore : IReadRepository, IWriteRepository, IUnitOfWork, IStockRepository
{
    private readonly Dictionary<Type, List<object>> data = new();

    public Task<T?> FindAsync<T>(Guid id) where T : class =>
        Task.FromResult(Items<T>().Cast<T>().FirstOrDefault(item => IdOf(item) == id));

    public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> predicate) where T : class =>
        Task.FromResult(Items<T>().Cast<T>().Where(predicate.Compile()).ToList());

    public void Add<T>(T entity) where T : class => Items<T>().Add(entity);

    public Task SaveAsync() => Task.CompletedTask;

    public Task<TResult> InTransactionAsync<TResult>(Func<Task<TResult>> operation) => operation();

    public async Task<bool> TryDecrementStockAsync(Guid id, int quantity)
    {
        var product = await FindAsync<Product>(id);
        if (product is null || product.Stock < quantity) return false;
        product.Stock -= quantity;
        return true;
    }

    private List<object> Items<T>() => data.TryGetValue(typeof(T), out var items)
        ? items
        : data[typeof(T)] = [];

    private static Guid IdOf<T>(T item) where T : class =>
        (Guid)typeof(T).GetProperty("Id")!.GetValue(item)!;
}
