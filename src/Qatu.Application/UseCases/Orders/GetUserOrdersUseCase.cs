using Qatu.Domain;

namespace Qatu.Application;

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
