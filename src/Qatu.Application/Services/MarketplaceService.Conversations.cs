using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<Conversation> OpenConversationAsync(Guid buyerId, Guid productId)
    {
        await UserAsync(buyerId);
        var product = await ProductAsync(productId);
        if (product.SellerId == buyerId) throw Forbidden();
        var existing = (await store.ListAsync<Conversation>(c => c.ProductId == productId && c.BuyerId == buyerId)).FirstOrDefault();
        if (existing is not null) return existing;
        var conversation = new Conversation { ProductId = productId, BuyerId = buyerId, SellerId = product.SellerId };
        store.Add(conversation);
        await store.SaveAsync();
        return conversation;
    }

    public async Task<List<Conversation>> ConversationsAsync(Guid userId)
    {
        await UserAsync(userId);
        return (await store.ListAsync<Conversation>(c => c.BuyerId == userId || c.SellerId == userId)).OrderByDescending(c => c.CreatedAt).ToList();
    }

    public async Task<Conversation> ConversationAsync(Guid userId, Guid id)
    {
        var conversation = await store.FindAsync<Conversation>(id) ?? throw NotFound();
        if (conversation.BuyerId != userId && conversation.SellerId != userId) throw Forbidden();
        return conversation;
    }

    public async Task<List<ChatMessage>> MessagesAsync(Guid userId, Guid conversationId)
    {
        await ConversationAsync(userId, conversationId);
        return (await store.ListAsync<ChatMessage>(m => m.ConversationId == conversationId)).OrderBy(m => m.SentAt).ToList();
    }

    public async Task<ChatMessage> SendMessageAsync(Guid userId, Guid conversationId, string text)
    {
        await ConversationAsync(userId, conversationId);
        if (string.IsNullOrWhiteSpace(text) || text.Trim().Length > 1000) throw Invalid("El mensaje debe tener entre 1 y 1000 caracteres.");
        var message = new ChatMessage { ConversationId = conversationId, SenderId = userId, Text = text.Trim() };
        store.Add(message);
        await store.SaveAsync();
        return message;
    }
}
