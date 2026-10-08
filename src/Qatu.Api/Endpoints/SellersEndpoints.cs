using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class SellersEndpoints
{
    public static void MapSellers(this RouteGroupBuilder secured)
    {
        secured.MapPost("/seller-applications", (ClaimsPrincipal user, SellerApplicationInput input, IMarketplaceService market) =>
            market.ApplySellerAsync(user.Id(), input.ShopName, input.Description));

        secured.MapGet("/seller-applications", (ClaimsPrincipal user, IMarketplaceService market) => market.ApplicationsAsync(user.Id()));

        secured.MapPost("/seller-applications/{id:guid}/decision", (ClaimsPrincipal user, Guid id, DecisionInput input, IMarketplaceService market) =>
            market.DecideApplicationAsync(user.Id(), id, input.Approve));

        secured.MapGet("/shops/mine", (ClaimsPrincipal user, IMarketplaceService market) => market.MyShopAsync(user.Id()));
    }
}
