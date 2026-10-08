namespace Qatu.Application;

public sealed class MarketException(MarketError kind, string message) : Exception(message)
{
    public MarketError Kind { get; } = kind;
}
