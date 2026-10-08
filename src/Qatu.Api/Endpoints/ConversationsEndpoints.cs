using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;

namespace Qatu.Api.Endpoints;

public static class ConversationsEndpoints
{
    public static void MapConversations(this RouteGroupBuilder secured)
    {
        secured.MapPost("/conversations", (
            ClaimsPrincipal user,
            ConversationInput input,
            OpenConversationUseCase useCase) =>
            useCase.ExecuteAsync(new OpenConversationCommand(user.Id(), input.ProductId)));

        secured.MapGet("/conversations", (ClaimsPrincipal user, ListConversationsUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));

        secured.MapGet("/conversations/{id:guid}/messages", (
            ClaimsPrincipal user,
            Guid id,
            GetConversationMessagesUseCase useCase) =>
            useCase.ExecuteAsync(new GetConversationMessagesQuery(user.Id(), id)));

        secured.MapPost("/conversations/{id:guid}/messages", async (
            ClaimsPrincipal user,
            Guid id,
            MessageInput input,
            SendConversationMessageUseCase useCase,
            IHubContext<ChatHub> hub) =>
        {
            var message = await useCase.ExecuteAsync(
                new SendConversationMessageCommand(user.Id(), id, input.Text));
            await hub.Clients.Group(ChatHub.Group(id)).SendAsync("message", message);
            return message;
        });
    }
}
