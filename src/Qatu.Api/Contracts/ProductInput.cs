namespace Qatu.Api.Contracts;

public sealed record ProductInput(string Name, string Description, string Category, decimal PriceBob, int Stock,
    string? Brand = null, string? Presentation = null, string? Unit = null);
