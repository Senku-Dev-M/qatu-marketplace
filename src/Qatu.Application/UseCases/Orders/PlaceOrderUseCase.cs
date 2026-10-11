using Qatu.Domain;

namespace Qatu.Application;

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

            await ApplyPaymentResultAsync(order, command, stock);
            writer.Add(order);
            await unitOfWork.SaveAsync();
            return order;
        });
    }

    private static async Task ApplyPaymentResultAsync(
        PurchaseOrder order,
        PlaceOrderCommand command,
        IStockRepository stock)
    {
        if (!command.PaymentSucceeds)
        {
            order.Status = OrderStatus.Failed;
            return;
        }

        if (!await stock.TryDecrementStockAsync(command.ProductId, command.Quantity))
            throw MarketErrors.Conflict("No hay existencias suficientes.");

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
    }
}
