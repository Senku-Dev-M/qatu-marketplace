using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<List<ProductCard>> CatalogAsync(string? query, string? category, Guid? sellerId = null, bool includeDrafts = false)
    {
        var products = await store.ListAsync<Product>(p => true);
        var shops = await store.ListAsync<Shop>(s => true);
        return products.Where(p => (includeDrafts || p.Status == ProductStatus.Published)
                && (sellerId is null || p.SellerId == sellerId)
                && (string.IsNullOrWhiteSpace(query) || p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || p.Brand.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || p.Presentation.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || p.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrWhiteSpace(category) || p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductCard(p.Id, p.Name, p.Brand, p.Presentation, p.Unit,
                p.Description, p.Category, p.PriceBob, p.Stock,
                p.Status.ToString(), shops.FirstOrDefault(s => s.Id == p.ShopId)?.Name ?? "Tienda", p.SellerId, p.ImagePath is not null))
            .ToList();
    }

    public async Task<Product> ProductAsync(Guid id, Guid? viewerId = null)
    {
        var product = await store.FindAsync<Product>(id) ?? throw NotFound();
        if (product.Status != ProductStatus.Published && product.SellerId != viewerId) throw NotFound();
        return product;
    }

    public async Task<(Stream Data, string ContentType)?> ProductImageAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await ProductAsync(productId);
        return product.ImagePath is null ? null : await images.ReadAsync(product.ImagePath, cancellationToken);
    }
}
