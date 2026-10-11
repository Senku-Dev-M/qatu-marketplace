using Qatu.Domain;

namespace Qatu.Application;

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
                && MatchesCategory(product, query.Category))
            .OrderByDescending(product => product.CreatedAt)
            .Select(product => ToCard(product, shops))
            .ToList();
    }

    private static bool MatchesQuery(Product product, string? search) =>
        string.IsNullOrWhiteSpace(search)
        || product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
        || product.Brand.Contains(search, StringComparison.OrdinalIgnoreCase)
        || product.Presentation.Contains(search, StringComparison.OrdinalIgnoreCase)
        || product.Description.Contains(search, StringComparison.OrdinalIgnoreCase);

    private static bool MatchesCategory(Product product, string? category) =>
        string.IsNullOrWhiteSpace(category)
        || product.Category.Equals(category, StringComparison.OrdinalIgnoreCase);

    private static ProductCard ToCard(Product product, List<Shop> shops) => new(
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
        product.ImagePath is not null);
}
