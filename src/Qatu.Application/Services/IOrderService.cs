using Qatu.Domain;

namespace Qatu.Application;

public interface IOrderService
{
    Task<PurchaseOrder> BuyAsync(Guid buyerId, Guid productId, int quantity, bool paymentSucceeds);
    Task<List<PurchaseOrder>> OrdersAsync(Guid userId);
}
