using Qatu.Domain;

namespace Qatu.Application;

public sealed class OrderService(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    IStockRepository stock,
    IUserAccessService users) : IOrderService
{
    public async Task<PurchaseOrder> BuyAsync(Guid buyerId, Guid productId, int quantity, bool paymentSucceeds)
    {
        await users.GetAsync(buyerId);
        if (quantity is < 1 or > 99) throw MarketErrors.Invalid("La cantidad debe estar entre 1 y 99.");
        return await unitOfWork.InTransactionAsync(async () =>
        {
            var product = await reader.FindAsync<Product>(productId) ?? throw MarketErrors.NotFound();
            if (product.Status != ProductStatus.Published || product.SellerId == buyerId) throw MarketErrors.Forbidden();
            var order = new PurchaseOrder
            {
                BuyerId = buyerId,
                SellerId = product.SellerId,
                ProductId = productId,
                Quantity = quantity,
                UnitPriceBob = product.PriceBob
            };
            if (paymentSucceeds)
            {
                if (!await stock.TryDecrementStockAsync(productId, quantity))
                    throw MarketErrors.Conflict("No hay existencias suficientes.");
                order.Status = OrderStatus.Paid;
                order.PaidAt = DateTime.UtcNow;
            }
            else order.Status = OrderStatus.Failed;
            writer.Add(order);
            await unitOfWork.SaveAsync();
            return order;
        });
    }

    public async Task<List<PurchaseOrder>> OrdersAsync(Guid userId)
    {
        await users.GetAsync(userId);
        return (await reader.ListAsync<PurchaseOrder>(order => order.BuyerId == userId || order.SellerId == userId))
            .OrderByDescending(order => order.CreatedAt).ToList();
    }
}
