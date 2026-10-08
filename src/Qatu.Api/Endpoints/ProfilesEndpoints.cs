using System.Security.Claims;
using Qatu.Application;

namespace Qatu.Api.Endpoints;

public static class ProfilesEndpoints
{
    public static void MapProfiles(this RouteGroupBuilder secured)
    {
        secured.MapGet("/me", (ClaimsPrincipal user, GetCurrentProfileUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));
    }
}
