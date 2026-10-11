using Qatu.Domain;

namespace Qatu.Application;

public sealed record CreateReviewCommand(
    Guid BuyerId,
    Guid OrderId,
    ReviewKind Kind,
    int Stars,
    string Comment);
