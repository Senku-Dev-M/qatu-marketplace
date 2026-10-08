using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Qatu.Application;

namespace Qatu.Api;

[Authorize]
public sealed class ChatHub(IConversationService conversations) : Hub
{
    public static string Group(Guid conversationId) => $"conversation:{conversationId:N}";

    public async Task Join(Guid conversationId)
    {
        var userId = Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await conversations.GetAsync(userId, conversationId);
        await Groups.AddToGroupAsync(Context.ConnectionId, Group(conversationId));
    }

    public Task Leave(Guid conversationId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(conversationId));
}
