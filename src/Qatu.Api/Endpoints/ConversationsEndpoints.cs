using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ConversationsEndpoints
{
    public static void MapConversations(this RouteGroupBuilder secured)
    {
        secured.MapPost("/conversations", (ClaimsPrincipal user, ConversationInput input, IMarketplaceService market) =>
            market.OpenConversationAsync(user.Id(), input.ProductId));

        secured.MapGet("/conversations", (ClaimsPrincipal user, IMarketplaceService market) => market.ConversationsAsync(user.Id()));

        secured.MapGet("/conversations/{id:guid}/messages", (ClaimsPrincipal user, Guid id, IMarketplaceService market) => market.MessagesAsync(user.Id(), id));

        secured.MapPost("/conversations/{id:guid}/messages", async (ClaimsPrincipal user, Guid id, MessageInput input, IMarketplaceService market, IHubContext<ChatHub> hub) =>
        {
            var message = await market.SendMessageAsync(user.Id(), id, input.Text);
            await hub.Clients.Group(ChatHub.Group(id)).SendAsync("message", message);
            return message;
        });
    }
}
