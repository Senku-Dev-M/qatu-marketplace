namespace Qatu.Api.Contracts;

public sealed record ReviewInput(Guid OrderId, string Kind, int Stars, string Comment);
