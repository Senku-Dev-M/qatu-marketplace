using Qatu.Application;
using Qatu.Tests.Support;

namespace Qatu.Tests.Products;

public sealed class SellerProductTests
{
    [Fact]
    public async Task SellerRequiresApprovalAndCanOnlyEditOwnProducts()
    {
        var scenario = new MarketplaceScenario();
        var denied = await Assert.ThrowsAsync<MarketException>(() =>
            scenario.CreateProduct.ExecuteAsync(new CreateProductCommand(
                scenario.Seller.Id,
                MarketplaceScenario.ProductData("Vaso", 10, 1))));
        Assert.Equal(MarketError.Forbidden, denied.Kind);

        var application = await scenario.ApplyAsSeller.ExecuteAsync(
            new ApplyAsSellerCommand(scenario.Seller.Id, "Tienda", "Cerámica"));
        await scenario.DecideSellerApplication.ExecuteAsync(
            new DecideSellerApplicationCommand(scenario.Admin.Id, application.Id, true));
        Assert.True(scenario.Seller.IsSeller);

        var product = await scenario.CreateProduct.ExecuteAsync(new CreateProductCommand(
            scenario.Seller.Id,
            MarketplaceScenario.ProductData("Vaso", 10, 2)));
        var other = await Assert.ThrowsAsync<MarketException>(() =>
            scenario.UpdateProduct.ExecuteAsync(new UpdateProductCommand(
                scenario.Buyer.Id,
                product.Id,
                MarketplaceScenario.ProductData("Otro", 10, 1))));
        Assert.Equal(MarketError.Forbidden, other.Kind);
    }
}
