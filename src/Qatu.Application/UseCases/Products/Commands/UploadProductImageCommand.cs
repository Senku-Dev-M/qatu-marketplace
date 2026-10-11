namespace Qatu.Application;

public sealed record UploadProductImageCommand(
    Guid SellerId,
    Guid ProductId,
    Stream Data,
    string ContentType);
