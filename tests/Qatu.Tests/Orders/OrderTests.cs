using Qatu.Application;
using Qatu.Domain;
using Qatu.Tests.Support;

namespace Qatu.Tests.Orders;

public sealed class OrderTests
{
    [Fact]
    public async Task FailedPaymentKeepsStockAndCannotBeReviewed()
    {
        var scenario = new MarketplaceScenario();
        var product = await scenario.PublishedProductAsync(3);
        var order = await scenario.PlaceOrder.ExecuteAsync(
            new PlaceOrderCommand(scenario.Buyer.Id, product.Id, 2, false));

        Assert.Equal(OrderStatus.Failed, order.Status);
        Assert.Equal(3, product.Stock);
        var denied = await Assert.ThrowsAsync<MarketException>(() =>
            scenario.CreateReview.ExecuteAsync(new CreateReviewCommand(
                scenario.Buyer.Id,
                order.Id,
                ReviewKind.Product,
                5,
                "Bien")));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
    }

    [Fact]
    public async Task PaidOrderDecrementsStockAndAllowsOneReview()
    {
        var scenario = new MarketplaceScenario();
        var product = await scenario.PublishedProductAsync(2);
        var order = await scenario.PlaceOrder.ExecuteAsync(
            new PlaceOrderCommand(scenario.Buyer.Id, product.Id, 2, true));

        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(0, product.Stock);
        var review = await scenario.CreateReview.ExecuteAsync(new CreateReviewCommand(
            scenario.Buyer.Id,
            order.Id,
            ReviewKind.Product,
            4,
            "Útil"));
        Assert.Equal(4, review.Stars);

        var duplicate = await Assert.ThrowsAsync<MarketException>(() =>
            scenario.CreateReview.ExecuteAsync(new CreateReviewCommand(
                scenario.Buyer.Id,
                order.Id,
                ReviewKind.Product,
                4,
                "Otra")));
        Assert.Equal(MarketError.Conflict, duplicate.Kind);

        var noStock = await Assert.ThrowsAsync<MarketException>(() =>
            scenario.PlaceOrder.ExecuteAsync(
                new PlaceOrderCommand(scenario.Buyer.Id, product.Id, 1, true)));
        Assert.Equal(MarketError.Conflict, noStock.Kind);
    }
}
