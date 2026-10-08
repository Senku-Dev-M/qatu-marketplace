using Qatu.Domain;

namespace Qatu.Application;

public sealed class ConversationAccessPolicy(IReadRepository reader)
{
    public async Task<Conversation> GetForParticipantAsync(Guid userId, Guid conversationId)
    {
        var conversation = await reader.FindAsync<Conversation>(conversationId) ?? throw MarketErrors.NotFound();
        return conversation.BuyerId == userId || conversation.SellerId == userId
            ? conversation
            : throw MarketErrors.Forbidden();
    }
}
