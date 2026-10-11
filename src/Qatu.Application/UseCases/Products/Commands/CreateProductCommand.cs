namespace Qatu.Application;

public sealed record CreateProductCommand(Guid SellerId, ProductData Product);
