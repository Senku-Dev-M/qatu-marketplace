namespace Qatu.Application;

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
