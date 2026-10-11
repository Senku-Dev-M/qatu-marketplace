namespace Qatu.Application;

public sealed record UpdateProductCommand(Guid SellerId, Guid ProductId, ProductData Product);
