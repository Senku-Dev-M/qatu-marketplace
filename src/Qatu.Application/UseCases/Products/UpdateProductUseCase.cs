using Qatu.Domain;

namespace Qatu.Application;

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
