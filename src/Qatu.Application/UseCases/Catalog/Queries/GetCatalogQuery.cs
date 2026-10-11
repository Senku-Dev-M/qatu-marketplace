namespace Qatu.Application;

public sealed record GetCatalogQuery(
    string? Search,
    string? Category,
    Guid? SellerId = null,
    bool IncludeDrafts = false);
