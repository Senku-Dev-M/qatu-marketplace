using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Qatu.Application;

namespace Qatu.Infrastructure;

public sealed class SupabaseImageStore(HttpClient client, SupabaseStorageOptions options) : IImageStore
{
    private const string Bucket = "qatu-products";
    private bool _bucketReady;

    private void Authorize(HttpRequestMessage request)
    {
        request.Headers.Add("apikey", options.ServiceRoleKey);
        // Managed projects use sb_secret_ keys in apikey; legacy local service_role JWTs
        // still need the Bearer header. A non-JWT secret must never be sent as Bearer.
        if (!options.ServiceRoleKey.StartsWith("sb_secret_", StringComparison.Ordinal))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ServiceRoleKey);
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (_bucketReady) return;
        using var check = new HttpRequestMessage(HttpMethod.Get, $"storage/v1/bucket/{Bucket}");
        Authorize(check);
        using var response = await client.SendAsync(check, cancellationToken);
        var missing = response.StatusCode == HttpStatusCode.NotFound ||
            (response.StatusCode == HttpStatusCode.BadRequest &&
             (await response.Content.ReadAsStringAsync(cancellationToken)).Contains("NoSuchBucket", StringComparison.Ordinal));
        if (missing)
        {
            using var create = new HttpRequestMessage(HttpMethod.Post, "storage/v1/bucket")
            { Content = JsonContent.Create(new { id = Bucket, name = Bucket, @public = false }) };
            Authorize(create);
            using var created = await client.SendAsync(create, cancellationToken);
            if (!created.IsSuccessStatusCode && created.StatusCode != HttpStatusCode.Conflict)
                throw new HttpRequestException($"No se pudo crear el bucket de imágenes: {created.StatusCode}");
        }
        else response.EnsureSuccessStatusCode();
        _bucketReady = true;
    }

    public async Task<string> UploadAsync(Guid productId, Stream data, string contentType, CancellationToken cancellationToken)
    {
        await EnsureBucketAsync(cancellationToken);
        var extension = contentType switch { "image/jpeg" => "jpg", "image/png" => "png", _ => "webp" };
        var path = $"{productId}/{Guid.NewGuid():N}.{extension}";
        using var request = new HttpRequestMessage(HttpMethod.Post, $"storage/v1/object/{Bucket}/{path}")
        { Content = new StreamContent(data) };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        Authorize(request);
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return path;
    }

    public async Task<(Stream Data, string ContentType)?> ReadAsync(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"storage/v1/object/authenticated/{Bucket}/{path}");
        Authorize(request);
        using var response = await client.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return (new MemoryStream(bytes), response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
    }
}
