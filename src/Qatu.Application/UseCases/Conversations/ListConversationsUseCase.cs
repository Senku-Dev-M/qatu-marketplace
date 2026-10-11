using Qatu.Domain;

namespace Qatu.Application;

public sealed class ListConversationsUseCase(IReadRepository reader, UserAccessPolicy users)
{
    public async Task<List<Conversation>> ExecuteAsync(Guid userId)
    {
        await users.GetAsync(userId);
        return (await reader.ListAsync<Conversation>(conversation =>
                conversation.BuyerId == userId || conversation.SellerId == userId))
            .OrderByDescending(conversation => conversation.CreatedAt)
            .ToList();
    }
}
