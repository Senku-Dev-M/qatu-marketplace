using Qatu.Application;
using Qatu.Tests.Support;

namespace Qatu.Tests.Conversations;

public sealed class ConversationTests
{
    [Fact]
    public async Task ChatHistoryRequiresParticipant()
    {
        var scenario = new MarketplaceScenario();
        var product = await scenario.PublishedProductAsync(2);
        var conversation = await scenario.OpenConversation.ExecuteAsync(
            new OpenConversationCommand(scenario.Buyer.Id, product.Id));
        await scenario.SendMessage.ExecuteAsync(
            new SendConversationMessageCommand(scenario.Buyer.Id, conversation.Id, "Hola"));

        Assert.Single(await scenario.GetMessages.ExecuteAsync(
            new GetConversationMessagesQuery(scenario.Seller.Id, conversation.Id)));
        var denied = await Assert.ThrowsAsync<MarketException>(() =>
            scenario.GetMessages.ExecuteAsync(
                new GetConversationMessagesQuery(scenario.Admin.Id, conversation.Id)));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
    }
}
