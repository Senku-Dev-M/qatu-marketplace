using Qatu.Domain;

namespace Qatu.Application;

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
