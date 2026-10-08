namespace Qatu.Application;

public interface IStockRepository
{
    Task<bool> TryDecrementStockAsync(Guid productId, int quantity);
}
