namespace Qatu.Application;

public sealed record PlaceOrderCommand(Guid BuyerId, Guid ProductId, int Quantity, bool PaymentSucceeds);
