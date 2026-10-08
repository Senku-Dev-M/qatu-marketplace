using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalog(this RouteGroupBuilder api)
    {
        api.MapGet("/catalog", (ICatalogService catalog, string? q, string? category) => catalog.CatalogAsync(q, category));

        api.MapGet("/products/{id:guid}", async (Guid id, ICatalogService catalog, IReviewService reviews) =>
        {
            var product = await catalog.ProductAsync(id);
            var cards = await catalog.CatalogAsync(null, null);
            return Results.Ok(new
            {
                product,
                shopName = cards.FirstOrDefault(p => p.Id == id)?.ShopName,
                reviews = await reviews.ProductReviewsAsync(id)
            });
        });

        api.MapGet("/products/{id:guid}/image", async (Guid id, ICatalogService catalog, CancellationToken cancellationToken) =>
        {
            var image = await catalog.ProductImageAsync(id, cancellationToken);
            return image is null ? Results.NotFound() : Results.File(image.Value.Data, image.Value.ContentType);
        });
    }
}
