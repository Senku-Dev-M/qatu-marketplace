using Qatu.Domain;

namespace Qatu.Application;

public sealed class ConversationService(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    IUserAccessService users,
    ICatalogService catalog) : IConversationService
{
    public async Task<Conversation> OpenAsync(Guid buyerId, Guid productId)
    {
        await users.GetAsync(buyerId);
        var product = await catalog.ProductAsync(productId);
        if (product.SellerId == buyerId) throw MarketErrors.Forbidden();
        var existing = (await reader.ListAsync<Conversation>(item => item.ProductId == productId && item.BuyerId == buyerId)).FirstOrDefault();
        if (existing is not null) return existing;
        var conversation = new Conversation { ProductId = productId, BuyerId = buyerId, SellerId = product.SellerId };
        writer.Add(conversation);
        await unitOfWork.SaveAsync();
        return conversation;
    }

    public async Task<List<Conversation>> ConversationsAsync(Guid userId)
    {
        await users.GetAsync(userId);
        return (await reader.ListAsync<Conversation>(item => item.BuyerId == userId || item.SellerId == userId))
            .OrderByDescending(item => item.CreatedAt).ToList();
    }

    public async Task<Conversation> GetAsync(Guid userId, Guid id)
    {
        var conversation = await reader.FindAsync<Conversation>(id) ?? throw MarketErrors.NotFound();
        return conversation.BuyerId == userId || conversation.SellerId == userId
            ? conversation
            : throw MarketErrors.Forbidden();
    }

    public async Task<List<ChatMessage>> MessagesAsync(Guid userId, Guid conversationId)
    {
        await GetAsync(userId, conversationId);
        return (await reader.ListAsync<ChatMessage>(message => message.ConversationId == conversationId))
            .OrderBy(message => message.SentAt).ToList();
    }

    public async Task<ChatMessage> SendAsync(Guid userId, Guid conversationId, string text)
    {
        await GetAsync(userId, conversationId);
        if (string.IsNullOrWhiteSpace(text) || text.Trim().Length > 1000)
            throw MarketErrors.Invalid("El mensaje debe tener entre 1 y 1000 caracteres.");
        var message = new ChatMessage { ConversationId = conversationId, SenderId = userId, Text = text.Trim() };
        writer.Add(message);
        await unitOfWork.SaveAsync();
        return message;
    }
}
