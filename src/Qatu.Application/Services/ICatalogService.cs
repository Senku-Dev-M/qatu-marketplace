using Qatu.Domain;

namespace Qatu.Application;

public interface ICatalogService
{
    Task<List<ProductCard>> CatalogAsync(string? query, string? category, Guid? sellerId = null, bool includeDrafts = false);
    Task<Product> ProductAsync(Guid id, Guid? viewerId = null);
    Task<(Stream Data, string ContentType)?> ProductImageAsync(Guid productId, CancellationToken cancellationToken);
}
