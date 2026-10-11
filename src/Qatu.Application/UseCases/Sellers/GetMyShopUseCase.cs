using Qatu.Domain;

namespace Qatu.Application;

public sealed class GetMyShopUseCase(IReadRepository reader, UserAccessPolicy users)
{
    public async Task<Shop?> ExecuteAsync(Guid sellerId)
    {
        await users.RequireSellerAsync(sellerId);
        return (await reader.ListAsync<Shop>(shop => shop.OwnerId == sellerId)).FirstOrDefault();
    }
}
