using Qatu.Domain;

namespace Qatu.Application;

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
