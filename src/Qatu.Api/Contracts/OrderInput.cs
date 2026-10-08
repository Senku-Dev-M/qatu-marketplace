namespace Qatu.Api.Contracts;

public sealed record OrderInput(Guid ProductId, int Quantity, bool PaymentSucceeds);
