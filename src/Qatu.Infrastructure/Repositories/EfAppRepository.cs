using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Infrastructure;

public sealed class EfAppRepository(AppDbContext db) : IAppRepository
{
    public Task<T?> FindAsync<T>(Guid id) where T : class => db.Set<T>().FindAsync(id).AsTask();
    public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> predicate) where T : class => db.Set<T>().Where(predicate).ToListAsync();
    public void Add<T>(T entity) where T : class => db.Set<T>().Add(entity);
    public Task SaveAsync() => db.SaveChangesAsync();

    public async Task<TResult> InTransactionAsync<TResult>(Func<Task<TResult>> operation)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        var result = await operation();
        await transaction.CommitAsync();
        return result;
    }

    public async Task<bool> TryDecrementStockAsync(Guid productId, int quantity)
    {
        var rows = await db.Products.Where(p => p.Id == productId && p.Status == ProductStatus.Published && p.Stock >= quantity)
            .ExecuteUpdateAsync(update => update.SetProperty(p => p.Stock, p => p.Stock - quantity));
        return rows == 1;
    }
}
