using Qatu.Domain;

namespace Qatu.Application;

public interface IProductService
{
    Task<Product> SaveAsync(Guid sellerId, Guid? productId, string name, string description,
        string category, decimal priceBob, int stock, string? brand = null, string? presentation = null, string? unit = null);
    Task<Product> SetStatusAsync(Guid sellerId, Guid productId, ProductStatus status);
    Task<Product> UploadImageAsync(Guid sellerId, Guid productId, Stream data, string contentType, CancellationToken cancellationToken);
}
