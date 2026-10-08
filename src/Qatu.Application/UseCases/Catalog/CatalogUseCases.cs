using Qatu.Domain;

namespace Qatu.Application;

public sealed record GetCatalogQuery(
    string? Search,
    string? Category,
    Guid? SellerId = null,
    bool IncludeDrafts = false);

public sealed record ProductDetails(Product Product, string ShopName, List<Review> Reviews);

public sealed class GetCatalogUseCase(IReadRepository reader)
{
    public async Task<List<ProductCard>> ExecuteAsync(GetCatalogQuery query)
    {
        var products = await reader.ListAsync<Product>(_ => true);
        var shops = await reader.ListAsync<Shop>(_ => true);
        return products
            .Where(product =>
                (query.IncludeDrafts || product.Status == ProductStatus.Published)
                && (query.SellerId is null || product.SellerId == query.SellerId)
                && MatchesQuery(product, query.Search)
                && (string.IsNullOrWhiteSpace(query.Category)
                    || product.Category.Equals(query.Category, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(product => product.CreatedAt)
            .Select(product => new ProductCard(
                product.Id,
                product.Name,
                product.Brand,
                product.Presentation,
                product.Unit,
                product.Description,
                product.Category,
                product.PriceBob,
                product.Stock,
                product.Status.ToString(),
                shops.FirstOrDefault(shop => shop.Id == product.ShopId)?.Name ?? "Tienda",
                product.SellerId,
                product.ImagePath is not null))
            .ToList();
    }

    private static bool MatchesQuery(Product product, string? search) =>
        string.IsNullOrWhiteSpace(search)
        || product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
        || product.Brand.Contains(search, StringComparison.OrdinalIgnoreCase)
        || product.Presentation.Contains(search, StringComparison.OrdinalIgnoreCase)
        || product.Description.Contains(search, StringComparison.OrdinalIgnoreCase);
}

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

public sealed class GetProductImageUseCase(ProductAccessPolicy products, IImageStore images)
{
    public async Task<(Stream Data, string ContentType)?> ExecuteAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await products.GetVisibleAsync(productId);
        return product.ImagePath is null
            ? null
            : await images.ReadAsync(product.ImagePath, cancellationToken);
    }
}
