namespace Qatu.Application;

public sealed class GetSellerProductsUseCase(GetCatalogUseCase catalog)
{
    public Task<List<ProductCard>> ExecuteAsync(Guid sellerId) =>
        catalog.ExecuteAsync(new GetCatalogQuery(null, null, sellerId, IncludeDrafts: true));
}
