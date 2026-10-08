using Qatu.Domain;

namespace Qatu.Application;

public sealed class ProductAccessPolicy(IReadRepository reader)
{
    public async Task<Product> GetVisibleAsync(Guid productId, Guid? viewerId = null)
    {
        var product = await reader.FindAsync<Product>(productId) ?? throw MarketErrors.NotFound();
        if (product.Status != ProductStatus.Published && product.SellerId != viewerId)
            throw MarketErrors.NotFound();
        return product;
    }

    public async Task<Product> GetOwnedAsync(Guid sellerId, Guid productId)
    {
        var product = await reader.FindAsync<Product>(productId) ?? throw MarketErrors.NotFound();
        return product.SellerId == sellerId ? product : throw MarketErrors.Forbidden();
    }
}
