using System.Security.Claims;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProducts(this RouteGroupBuilder secured)
    {
        secured.MapGet("/products/mine", (ClaimsPrincipal user, GetSellerProductsUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));

        secured.MapPost("/products", (ClaimsPrincipal user, ProductInput input, CreateProductUseCase useCase) =>
            useCase.ExecuteAsync(new CreateProductCommand(user.Id(), ToProductData(input))));

        secured.MapPut("/products/{id:guid}", (
            ClaimsPrincipal user,
            Guid id,
            ProductInput input,
            UpdateProductUseCase useCase) =>
            useCase.ExecuteAsync(new UpdateProductCommand(user.Id(), id, ToProductData(input))));

        secured.MapPost("/products/{id:guid}/status", (
            ClaimsPrincipal user,
            Guid id,
            ProductStatusInput input,
            ChangeProductStatusUseCase useCase) =>
        {
            if (!Enum.TryParse<ProductStatus>(input.Status, true, out var status) || !Enum.IsDefined(status)) throw new MarketException(MarketError.Invalid, "Estado de producto inválido.");
            return useCase.ExecuteAsync(new ChangeProductStatusCommand(user.Id(), id, status));
        });

        secured.MapPost("/products/{id:guid}/image", async (
            ClaimsPrincipal user,
            Guid id,
            IFormFile file,
            UploadProductImageUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            if (file.Length is 0 or > 5_000_000) throw new MarketException(MarketError.Invalid, "La imagen debe medir entre 1 byte y 5 MB.");
            await using var stream = file.OpenReadStream();
            return await useCase.ExecuteAsync(
                new UploadProductImageCommand(user.Id(), id, stream, file.ContentType),
                cancellationToken);
        }).DisableAntiforgery();
    }

    private static ProductData ToProductData(ProductInput input) => new(
        input.Name,
        input.Description,
        input.Category,
        input.PriceBob,
        input.Stock,
        input.Brand,
        input.Presentation,
        input.Unit);
}
