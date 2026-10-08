using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ProfilesEndpoints
{
    public static void MapProfiles(this RouteGroupBuilder secured)
    {
        secured.MapGet("/me", (ClaimsPrincipal user, IMarketplaceService market) => market.EnsureProfileAsync(user.Id(), user.Email(), user.Identity?.Name));
    }
}
