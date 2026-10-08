using System.Security.Claims;
using Qatu.Api.Contracts;
using Qatu.Application;

namespace Qatu.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrders(this RouteGroupBuilder secured)
    {
        secured.MapPost("/orders", (ClaimsPrincipal user, OrderInput input, PlaceOrderUseCase useCase) =>
            useCase.ExecuteAsync(new PlaceOrderCommand(
                user.Id(),
                input.ProductId,
                input.Quantity,
                input.PaymentSucceeds)));

        secured.MapGet("/orders", (ClaimsPrincipal user, GetUserOrdersUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));
    }
}
