namespace Qatu.Application;

public sealed partial class MarketplaceService(IAppRepository store, IImageStore images, MarketplaceOptions options) : IMarketplaceService
{
    private static MarketException Invalid(string message) => new(MarketError.Invalid, message);
    private static MarketException Forbidden() => new(MarketError.Forbidden, "No tienes permiso para esta acción.");
    private static MarketException NotFound() => new(MarketError.NotFound, "El recurso no existe.");

}
