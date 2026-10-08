using Qatu.Domain;

namespace Qatu.Application;

public sealed class ProductService(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    IUserAccessService users,
    IImageStore images) : IProductService
{
    public async Task<Product> SaveAsync(Guid sellerId, Guid? productId, string name, string description,
        string category, decimal priceBob, int stock, string? brand = null, string? presentation = null, string? unit = null)
    {
        await users.RequireSellerAsync(sellerId);
        Validate(name, category, priceBob, stock, brand, presentation, unit);
        var shop = (await reader.ListAsync<Shop>(item => item.OwnerId == sellerId)).FirstOrDefault() ?? throw MarketErrors.Forbidden();
        var product = productId is null
            ? new Product { ShopId = shop.Id, SellerId = sellerId }
            : await reader.FindAsync<Product>(productId.Value) ?? throw MarketErrors.NotFound();
        if (product.SellerId != sellerId) throw MarketErrors.Forbidden();

        product.Name = name.Trim();
        product.Description = description.Trim();
        product.Category = category.Trim();
        product.Brand = brand?.Trim() ?? "";
        product.Presentation = presentation?.Trim() ?? "";
        product.Unit = string.IsNullOrWhiteSpace(unit) ? "unidad" : unit.Trim();
        product.PriceBob = decimal.Round(priceBob, 2);
        product.Stock = stock;
        if (productId is null) writer.Add(product);
        await unitOfWork.SaveAsync();
        return product;
    }

    public async Task<Product> SetStatusAsync(Guid sellerId, Guid productId, ProductStatus status)
    {
        await users.RequireSellerAsync(sellerId);
        var product = await OwnedProductAsync(sellerId, productId);
        if (status == ProductStatus.Published && product.Stock == 0)
            throw MarketErrors.Invalid("Añade existencias antes de publicar.");
        product.Status = status;
        await unitOfWork.SaveAsync();
        return product;
    }

    public async Task<Product> UploadImageAsync(Guid sellerId, Guid productId, Stream data, string contentType, CancellationToken cancellationToken)
    {
        await users.RequireSellerAsync(sellerId);
        var product = await OwnedProductAsync(sellerId, productId);
        if (contentType is not ("image/jpeg" or "image/png" or "image/webp"))
            throw MarketErrors.Invalid("Usa una imagen JPG, PNG o WebP.");
        product.ImagePath = await images.UploadAsync(productId, data, contentType, cancellationToken);
        await unitOfWork.SaveAsync();
        return product;
    }

    private async Task<Product> OwnedProductAsync(Guid sellerId, Guid productId)
    {
        var product = await reader.FindAsync<Product>(productId) ?? throw MarketErrors.NotFound();
        return product.SellerId == sellerId ? product : throw MarketErrors.Forbidden();
    }

    private static void Validate(string name, string category, decimal priceBob, int stock, string? brand, string? presentation, string? unit)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 120 || string.IsNullOrWhiteSpace(category) || priceBob <= 0 || stock < 0)
            throw MarketErrors.Invalid("Revisa nombre, categoría, precio y existencias.");
        if (brand?.Length > 80 || presentation?.Length > 40 || unit?.Length > 30)
            throw MarketErrors.Invalid("Marca, presentación o unidad demasiado largas.");
    }
}
