using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<Product> SaveProductAsync(Guid sellerId, Guid? productId, string name, string description,
        string category, decimal priceBob, int stock, string? brand = null, string? presentation = null, string? unit = null)
    {
        await RequireSellerAsync(sellerId);
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 120 || string.IsNullOrWhiteSpace(category) || priceBob <= 0 || stock < 0)
            throw Invalid("Revisa nombre, categoría, precio y existencias.");
        if (brand?.Length > 80 || presentation?.Length > 40 || unit?.Length > 30)
            throw Invalid("Marca, presentación o unidad demasiado largas.");
        var shop = await MyShopAsync(sellerId) ?? throw Forbidden();
        var product = productId is null ? new Product { ShopId = shop.Id, SellerId = sellerId } : await store.FindAsync<Product>(productId.Value) ?? throw NotFound();
        if (product.SellerId != sellerId) throw Forbidden();
        product.Name = name.Trim(); product.Description = description.Trim(); product.Category = category.Trim();
        product.Brand = brand?.Trim() ?? "";
        product.Presentation = presentation?.Trim() ?? "";
        product.Unit = string.IsNullOrWhiteSpace(unit) ? "unidad" : unit.Trim();
        product.PriceBob = decimal.Round(priceBob, 2); product.Stock = stock;
        if (productId is null) store.Add(product);
        await store.SaveAsync();
        return product;
    }

    public async Task<Product> SetProductStatusAsync(Guid sellerId, Guid productId, ProductStatus status)
    {
        await RequireSellerAsync(sellerId);
        var product = await store.FindAsync<Product>(productId) ?? throw NotFound();
        if (product.SellerId != sellerId) throw Forbidden();
        if (status == ProductStatus.Published && product.Stock == 0) throw Invalid("Añade existencias antes de publicar.");
        product.Status = status;
        await store.SaveAsync();
        return product;
    }

    public async Task<Product> UploadProductImageAsync(Guid sellerId, Guid productId, Stream data, string contentType, CancellationToken cancellationToken)
    {
        await RequireSellerAsync(sellerId);
        var product = await store.FindAsync<Product>(productId) ?? throw NotFound();
        if (product.SellerId != sellerId) throw Forbidden();
        if (contentType is not ("image/jpeg" or "image/png" or "image/webp")) throw Invalid("Usa una imagen JPG, PNG o WebP.");
        product.ImagePath = await images.UploadAsync(productId, data, contentType, cancellationToken);
        await store.SaveAsync();
        return product;
    }
}
