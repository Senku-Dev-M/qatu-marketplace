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
        api.MapGet("/catalog", (IMarketplaceService market, string? q, string? category) => market.CatalogAsync(q, category));

        api.MapGet("/products/{id:guid}", async (Guid id, IMarketplaceService market) =>
        {
            var product = await market.ProductAsync(id);
            var cards = await market.CatalogAsync(null, null);
            return Results.Ok(new { product, shopName = cards.FirstOrDefault(p => p.Id == id)?.ShopName,
                reviews = await market.ProductReviewsAsync(id) });
        });

        api.MapGet("/products/{id:guid}/image", async (Guid id, IMarketplaceService market, CancellationToken cancellationToken) =>
        {
            var image = await market.ProductImageAsync(id, cancellationToken);
            return image is null ? Results.NotFound() : Results.File(image.Value.Data, image.Value.ContentType);
        });
    }
}
