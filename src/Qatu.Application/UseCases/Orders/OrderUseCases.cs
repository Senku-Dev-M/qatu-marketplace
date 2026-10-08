using Qatu.Domain;

namespace Qatu.Application;

public sealed record PlaceOrderCommand(Guid BuyerId, Guid ProductId, int Quantity, bool PaymentSucceeds);

public sealed class PlaceOrderUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    IStockRepository stock,
    UserAccessPolicy users)
{
    public async Task<PurchaseOrder> ExecuteAsync(PlaceOrderCommand command)
    {
        await users.GetAsync(command.BuyerId);
        if (command.Quantity is < 1 or > 99)
            throw MarketErrors.Invalid("La cantidad debe estar entre 1 y 99.");
        return await unitOfWork.InTransactionAsync(async () =>
        {
            var product = await reader.FindAsync<Product>(command.ProductId) ?? throw MarketErrors.NotFound();
            if (product.Status != ProductStatus.Published || product.SellerId == command.BuyerId)
                throw MarketErrors.Forbidden();
            var order = new PurchaseOrder
            {
                BuyerId = command.BuyerId,
                SellerId = product.SellerId,
                ProductId = command.ProductId,
                Quantity = command.Quantity,
                UnitPriceBob = product.PriceBob
            };
            if (command.PaymentSucceeds)
            {
                if (!await stock.TryDecrementStockAsync(command.ProductId, command.Quantity))
                    throw MarketErrors.Conflict("No hay existencias suficientes.");
                order.Status = OrderStatus.Paid;
                order.PaidAt = DateTime.UtcNow;
            }
            else
            {
                order.Status = OrderStatus.Failed;
            }
            writer.Add(order);
            await unitOfWork.SaveAsync();
            return order;
        });
    }
}

public sealed class GetUserOrdersUseCase(IReadRepository reader, UserAccessPolicy users)
{
    public async Task<List<PurchaseOrder>> ExecuteAsync(Guid userId)
    {
        await users.GetAsync(userId);
        return (await reader.ListAsync<PurchaseOrder>(order =>
                order.BuyerId == userId || order.SellerId == userId))
            .OrderByDescending(order => order.CreatedAt)
            .ToList();
    }
}
