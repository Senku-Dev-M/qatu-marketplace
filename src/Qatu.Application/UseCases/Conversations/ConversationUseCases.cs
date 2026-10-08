using Qatu.Domain;

namespace Qatu.Application;

public sealed record OpenConversationCommand(Guid BuyerId, Guid ProductId);
public sealed record GetConversationMessagesQuery(Guid UserId, Guid ConversationId);
public sealed record SendConversationMessageCommand(Guid UserId, Guid ConversationId, string Text);

public sealed class OpenConversationUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    UserAccessPolicy users,
    ProductAccessPolicy products)
{
    public async Task<Conversation> ExecuteAsync(OpenConversationCommand command)
    {
        await users.GetAsync(command.BuyerId);
        var product = await products.GetVisibleAsync(command.ProductId);
        if (product.SellerId == command.BuyerId) throw MarketErrors.Forbidden();
        var existing = (await reader.ListAsync<Conversation>(conversation =>
            conversation.ProductId == command.ProductId && conversation.BuyerId == command.BuyerId))
            .FirstOrDefault();
        if (existing is not null) return existing;
        var conversation = new Conversation
        {
            ProductId = command.ProductId,
            BuyerId = command.BuyerId,
            SellerId = product.SellerId
        };
        writer.Add(conversation);
        await unitOfWork.SaveAsync();
        return conversation;
    }
}

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

public sealed class AuthorizeConversationUseCase(ConversationAccessPolicy conversations)
{
    public Task<Conversation> ExecuteAsync(Guid userId, Guid conversationId) =>
        conversations.GetForParticipantAsync(userId, conversationId);
}

public sealed class GetConversationMessagesUseCase(
    IReadRepository reader,
    ConversationAccessPolicy conversations)
{
    public async Task<List<ChatMessage>> ExecuteAsync(GetConversationMessagesQuery query)
    {
        await conversations.GetForParticipantAsync(query.UserId, query.ConversationId);
        return (await reader.ListAsync<ChatMessage>(message =>
                message.ConversationId == query.ConversationId))
            .OrderBy(message => message.SentAt)
            .ToList();
    }
}

public sealed class SendConversationMessageUseCase(
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    ConversationAccessPolicy conversations)
{
    public async Task<ChatMessage> ExecuteAsync(SendConversationMessageCommand command)
    {
        await conversations.GetForParticipantAsync(command.UserId, command.ConversationId);
        if (string.IsNullOrWhiteSpace(command.Text) || command.Text.Trim().Length > 1000)
            throw MarketErrors.Invalid("El mensaje debe tener entre 1 y 1000 caracteres.");
        var message = new ChatMessage
        {
            ConversationId = command.ConversationId,
            SenderId = command.UserId,
            Text = command.Text.Trim()
        };
        writer.Add(message);
        await unitOfWork.SaveAsync();
        return message;
    }
}
