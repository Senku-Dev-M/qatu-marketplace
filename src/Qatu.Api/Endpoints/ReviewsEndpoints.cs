using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ReviewsEndpoints
{
    public static void MapReviews(this RouteGroupBuilder api, RouteGroupBuilder secured)
    {
        api.MapGet("/sellers/{id:guid}/reviews", (Guid id, IMarketplaceService market) => market.SellerReviewsAsync(id));

        secured.MapPost("/reviews", (ClaimsPrincipal user, ReviewInput input, IMarketplaceService market) =>
        {
            if (!Enum.TryParse<ReviewKind>(input.Kind, true, out var kind) || !Enum.IsDefined(kind)) throw new MarketException(MarketError.Invalid, "Tipo de calificación inválido.");
            return market.ReviewAsync(user.Id(), input.OrderId, kind, input.Stars, input.Comment);
        });
    }
}
