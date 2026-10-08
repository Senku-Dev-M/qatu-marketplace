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
        secured.MapPost("/seller-applications", (ClaimsPrincipal user, SellerApplicationInput input, ISellerService sellers) =>
            sellers.ApplyAsync(user.Id(), input.ShopName, input.Description));

        secured.MapGet("/seller-applications", (ClaimsPrincipal user, ISellerService sellers) => sellers.ApplicationsAsync(user.Id()));

        secured.MapPost("/seller-applications/{id:guid}/decision", (ClaimsPrincipal user, Guid id, DecisionInput input, ISellerService sellers) =>
            sellers.DecideAsync(user.Id(), id, input.Approve));

        secured.MapGet("/shops/mine", (ClaimsPrincipal user, ISellerService sellers) => sellers.MyShopAsync(user.Id()));
    }
}
