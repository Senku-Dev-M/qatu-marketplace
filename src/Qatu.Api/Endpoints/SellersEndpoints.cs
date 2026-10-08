using System.Security.Claims;
using Qatu.Api.Contracts;
using Qatu.Application;

namespace Qatu.Api.Endpoints;

public static class SellersEndpoints
{
    public static void MapSellers(this RouteGroupBuilder secured)
    {
        secured.MapPost("/seller-applications", (
            ClaimsPrincipal user,
            SellerApplicationInput input,
            ApplyAsSellerUseCase useCase) =>
            useCase.ExecuteAsync(new ApplyAsSellerCommand(user.Id(), input.ShopName, input.Description)));

        secured.MapGet("/seller-applications", (ClaimsPrincipal user, ListSellerApplicationsUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));

        secured.MapPost("/seller-applications/{id:guid}/decision", (
            ClaimsPrincipal user,
            Guid id,
            DecisionInput input,
            DecideSellerApplicationUseCase useCase) =>
            useCase.ExecuteAsync(new DecideSellerApplicationCommand(user.Id(), id, input.Approve)));

        secured.MapGet("/shops/mine", (ClaimsPrincipal user, GetMyShopUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));
    }
}
