using Qatu.Domain;

namespace Qatu.Application;

public sealed class GetProductDetailsUseCase(IReadRepository reader, ProductAccessPolicy products)
{
    public async Task<ProductDetails> ExecuteAsync(Guid productId)
    {
        var product = await products.GetVisibleAsync(productId);
        var shop = await reader.FindAsync<Shop>(product.ShopId);
        var reviews = await reader.ListAsync<Review>(review =>
            review.ProductId == productId && review.Kind == ReviewKind.Product);

        return new ProductDetails(product, shop?.Name ?? "Tienda", reviews);
    }
}
