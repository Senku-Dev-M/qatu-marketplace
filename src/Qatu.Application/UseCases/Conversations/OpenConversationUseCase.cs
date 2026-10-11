using Qatu.Domain;

namespace Qatu.Application;

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
