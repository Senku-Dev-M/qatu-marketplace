namespace Qatu.Application;

internal static class MarketErrors
{
    internal static MarketException Invalid(string message) => new(MarketError.Invalid, message);
    internal static MarketException Forbidden() => new(MarketError.Forbidden, "No tienes permiso para esta acción.");
    internal static MarketException NotFound() => new(MarketError.NotFound, "El recurso no existe.");
    internal static MarketException Conflict(string message) => new(MarketError.Conflict, message);
}
