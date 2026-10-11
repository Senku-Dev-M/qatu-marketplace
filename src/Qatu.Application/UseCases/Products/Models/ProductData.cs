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
