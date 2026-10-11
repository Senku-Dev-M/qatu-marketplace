using Qatu.Domain;

namespace Qatu.Application;

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
