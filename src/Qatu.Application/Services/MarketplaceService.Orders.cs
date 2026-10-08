using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<PurchaseOrder> BuyAsync(Guid buyerId, Guid productId, int quantity, bool paymentSucceeds)
    {
        await UserAsync(buyerId);
        if (quantity < 1 || quantity > 99) throw Invalid("La cantidad debe estar entre 1 y 99.");
        return await store.InTransactionAsync(async () =>
        {
            var product = await store.FindAsync<Product>(productId) ?? throw NotFound();
            if (product.Status != ProductStatus.Published || product.SellerId == buyerId) throw Forbidden();
            var order = new PurchaseOrder { BuyerId = buyerId, SellerId = product.SellerId, ProductId = productId, Quantity = quantity, UnitPriceBob = product.PriceBob };
            if (paymentSucceeds)
            {
                if (!await store.TryDecrementStockAsync(productId, quantity)) throw new MarketException(MarketError.Conflict, "No hay existencias suficientes.");
                order.Status = OrderStatus.Paid;
                order.PaidAt = DateTime.UtcNow;
            }
            else order.Status = OrderStatus.Failed;
            store.Add(order);
            await store.SaveAsync();
            return order;
        });
    }

    public async Task<List<PurchaseOrder>> OrdersAsync(Guid userId)
    {
        await UserAsync(userId);
        return (await store.ListAsync<PurchaseOrder>(o => o.BuyerId == userId || o.SellerId == userId)).OrderByDescending(o => o.CreatedAt).ToList();
    }
}
