using Qatu.Domain;

namespace Qatu.Application;

public sealed record ProductData(
    string Name,
    string Description,
    string Category,
    decimal PriceBob,
    int Stock,
    string? Brand = null,
    string? Presentation = null,
    string? Unit = null);

public sealed record CreateProductCommand(Guid SellerId, ProductData Product);
public sealed record UpdateProductCommand(Guid SellerId, Guid ProductId, ProductData Product);
public sealed record ChangeProductStatusCommand(Guid SellerId, Guid ProductId, ProductStatus Status);
public sealed record UploadProductImageCommand(
    Guid SellerId,
    Guid ProductId,
    Stream Data,
    string ContentType);

public sealed class GetSellerProductsUseCase(GetCatalogUseCase catalog)
{
    public Task<List<ProductCard>> ExecuteAsync(Guid sellerId) =>
        catalog.ExecuteAsync(new GetCatalogQuery(null, null, sellerId, IncludeDrafts: true));
}

public sealed class CreateProductUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    UserAccessPolicy users)
{
    public async Task<Product> ExecuteAsync(CreateProductCommand command)
    {
        await users.RequireSellerAsync(command.SellerId);
        ProductRules.Validate(command.Product);
        var shop = (await reader.ListAsync<Shop>(shop => shop.OwnerId == command.SellerId)).FirstOrDefault()
            ?? throw MarketErrors.Forbidden();
        var product = new Product { ShopId = shop.Id, SellerId = command.SellerId };
        ProductRules.Apply(product, command.Product);
        writer.Add(product);
        await unitOfWork.SaveAsync();
        return product;
    }
}

public sealed class UpdateProductUseCase(
    IUnitOfWork unitOfWork,
    UserAccessPolicy users,
    ProductAccessPolicy products)
{
    public async Task<Product> ExecuteAsync(UpdateProductCommand command)
    {
        await users.RequireSellerAsync(command.SellerId);
        ProductRules.Validate(command.Product);
        var product = await products.GetOwnedAsync(command.SellerId, command.ProductId);
        ProductRules.Apply(product, command.Product);
        await unitOfWork.SaveAsync();
        return product;
    }
}

public sealed class ChangeProductStatusUseCase(
    IUnitOfWork unitOfWork,
    UserAccessPolicy users,
    ProductAccessPolicy products)
{
    public async Task<Product> ExecuteAsync(ChangeProductStatusCommand command)
    {
        await users.RequireSellerAsync(command.SellerId);
        var product = await products.GetOwnedAsync(command.SellerId, command.ProductId);
        if (command.Status == ProductStatus.Published && product.Stock == 0)
            throw MarketErrors.Invalid("Añade existencias antes de publicar.");
        product.Status = command.Status;
        await unitOfWork.SaveAsync();
        return product;
    }
}

public sealed class UploadProductImageUseCase(
    IUnitOfWork unitOfWork,
    UserAccessPolicy users,
    ProductAccessPolicy products,
    IImageStore images)
{
    public async Task<Product> ExecuteAsync(
        UploadProductImageCommand command,
        CancellationToken cancellationToken)
    {
        await users.RequireSellerAsync(command.SellerId);
        var product = await products.GetOwnedAsync(command.SellerId, command.ProductId);
        if (command.ContentType is not ("image/jpeg" or "image/png" or "image/webp"))
            throw MarketErrors.Invalid("Usa una imagen JPG, PNG o WebP.");
        product.ImagePath = await images.UploadAsync(
            command.ProductId,
            command.Data,
            command.ContentType,
            cancellationToken);
        await unitOfWork.SaveAsync();
        return product;
    }
}

internal static class ProductRules
{
    internal static void Validate(ProductData data)
    {
        if (string.IsNullOrWhiteSpace(data.Name)
            || data.Name.Trim().Length > 120
            || string.IsNullOrWhiteSpace(data.Category)
            || data.PriceBob <= 0
            || data.Stock < 0)
            throw MarketErrors.Invalid("Revisa nombre, categoría, precio y existencias.");
        if (data.Brand?.Length > 80 || data.Presentation?.Length > 40 || data.Unit?.Length > 30)
            throw MarketErrors.Invalid("Marca, presentación o unidad demasiado largas.");
    }

    internal static void Apply(Product product, ProductData data)
    {
        product.Name = data.Name.Trim();
        product.Description = data.Description.Trim();
        product.Category = data.Category.Trim();
        product.Brand = data.Brand?.Trim() ?? "";
        product.Presentation = data.Presentation?.Trim() ?? "";
        product.Unit = string.IsNullOrWhiteSpace(data.Unit) ? "unidad" : data.Unit.Trim();
        product.PriceBob = decimal.Round(data.PriceBob, 2);
        product.Stock = data.Stock;
    }
}
