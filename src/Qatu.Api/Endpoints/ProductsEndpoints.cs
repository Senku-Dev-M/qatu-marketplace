using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProducts(this RouteGroupBuilder secured)
    {
        secured.MapGet("/products/mine", (ClaimsPrincipal user, ICatalogService catalog) => catalog.CatalogAsync(null, null, user.Id(), true));

        secured.MapPost("/products", (ClaimsPrincipal user, ProductInput input, IProductService products) =>
            products.SaveAsync(user.Id(), null, input.Name, input.Description, input.Category, input.PriceBob, input.Stock,
                input.Brand, input.Presentation, input.Unit));

        secured.MapPut("/products/{id:guid}", (ClaimsPrincipal user, Guid id, ProductInput input, IProductService products) =>
            products.SaveAsync(user.Id(), id, input.Name, input.Description, input.Category, input.PriceBob, input.Stock,
                input.Brand, input.Presentation, input.Unit));

        secured.MapPost("/products/{id:guid}/status", (ClaimsPrincipal user, Guid id, ProductStatusInput input, IProductService products) =>
        {
            if (!Enum.TryParse<ProductStatus>(input.Status, true, out var status) || !Enum.IsDefined(status)) throw new MarketException(MarketError.Invalid, "Estado de producto inválido.");
            return products.SetStatusAsync(user.Id(), id, status);
        });

        secured.MapPost("/products/{id:guid}/image", async (ClaimsPrincipal user, Guid id, IFormFile file, IProductService products, CancellationToken cancellationToken) =>
        {
            if (file.Length is 0 or > 5_000_000) throw new MarketException(MarketError.Invalid, "La imagen debe medir entre 1 byte y 5 MB.");
            await using var stream = file.OpenReadStream();
            return await products.UploadImageAsync(user.Id(), id, stream, file.ContentType, cancellationToken);
        }).DisableAntiforgery();
    }
}
