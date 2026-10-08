using Qatu.Application;

namespace Qatu.Api.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalog(this RouteGroupBuilder api)
    {
        api.MapGet("/catalog", (GetCatalogUseCase useCase, string? q, string? category) =>
            useCase.ExecuteAsync(new GetCatalogQuery(q, category)));

        api.MapGet("/products/{id:guid}", async (Guid id, GetProductDetailsUseCase useCase) =>
        {
            var details = await useCase.ExecuteAsync(id);
            return Results.Ok(new
            {
                product = details.Product,
                shopName = details.ShopName,
                reviews = details.Reviews
            });
        });

        api.MapGet("/products/{id:guid}/image", async (
            Guid id,
            GetProductImageUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var image = await useCase.ExecuteAsync(id, cancellationToken);
            return image is null ? Results.NotFound() : Results.File(image.Value.Data, image.Value.ContentType);
        });
    }
}
