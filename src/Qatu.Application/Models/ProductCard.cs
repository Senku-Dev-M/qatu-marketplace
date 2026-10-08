namespace Qatu.Application;

public sealed record ProductCard(Guid Id, string Name, string Brand, string Presentation, string Unit,
    string Description, string Category, decimal PriceBob, int Stock, string Status,
    string ShopName, Guid SellerId, bool HasImage);
