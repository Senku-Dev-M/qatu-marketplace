namespace Qatu.Application;

public interface IImageStore
{
    Task<string> UploadAsync(Guid productId, Stream data, string contentType, CancellationToken cancellationToken);
    Task<(Stream Data, string ContentType)?> ReadAsync(string path, CancellationToken cancellationToken);
}
