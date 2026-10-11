using Qatu.Domain;

namespace Qatu.Application;

public sealed record ChangeProductStatusCommand(Guid SellerId, Guid ProductId, ProductStatus Status);
