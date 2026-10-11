using System.Security.Claims;
using Qatu.Api.Contracts;
using Qatu.Api.Parsing;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ReviewsEndpoints
{
    public static void MapReviews(this RouteGroupBuilder api, RouteGroupBuilder secured)
    {
        api.MapGet("/sellers/{id:guid}/reviews", (Guid id, GetSellerReviewsUseCase useCase) =>
            useCase.ExecuteAsync(id));

        secured.MapPost("/reviews", (ClaimsPrincipal user, ReviewInput input, CreateReviewUseCase useCase) =>
        {
            var kind = ContractParser.ParseEnum<ReviewKind>(input.Kind, "Tipo de calificación inválido.");
            return useCase.ExecuteAsync(new CreateReviewCommand(
                user.Id(),
                input.OrderId,
                kind,
                input.Stars,
                input.Comment));
        });
    }
}
