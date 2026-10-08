using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrders(this RouteGroupBuilder secured)
    {
        secured.MapPost("/orders", (ClaimsPrincipal user, OrderInput input, IOrderService orders) =>
            orders.BuyAsync(user.Id(), input.ProductId, input.Quantity, input.PaymentSucceeds));

        secured.MapGet("/orders", (ClaimsPrincipal user, IOrderService orders) => orders.OrdersAsync(user.Id()));
    }
}
