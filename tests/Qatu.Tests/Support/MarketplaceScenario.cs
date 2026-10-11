using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Tests.Support;

internal sealed class MarketplaceScenario
{
    internal readonly MemoryStore Store = new();
    internal readonly ApplyAsSellerUseCase ApplyAsSeller;
    internal readonly DecideSellerApplicationUseCase DecideSellerApplication;
    internal readonly CreateProductUseCase CreateProduct;
    internal readonly UpdateProductUseCase UpdateProduct;
    internal readonly ChangeProductStatusUseCase ChangeProductStatus;
    internal readonly PlaceOrderUseCase PlaceOrder;
    internal readonly CreateReviewUseCase CreateReview;
    internal readonly OpenConversationUseCase OpenConversation;
    internal readonly SendConversationMessageUseCase SendMessage;
    internal readonly GetConversationMessagesUseCase GetMessages;
    internal readonly Profile Admin = new() { Email = "admin@test.local", IsAdmin = true };
    internal readonly Profile Seller = new() { Email = "seller@test.local" };
    internal readonly Profile Buyer = new() { Email = "buyer@test.local" };

    internal MarketplaceScenario()
    {
        Admin.Id = Guid.NewGuid();
        Seller.Id = Guid.NewGuid();
        Buyer.Id = Guid.NewGuid();
        Store.Add(Admin);
        Store.Add(Seller);
        Store.Add(Buyer);

        var users = new UserAccessPolicy(Store);
        var products = new ProductAccessPolicy(Store);
        var conversations = new ConversationAccessPolicy(Store);
        ApplyAsSeller = new ApplyAsSellerUseCase(Store, Store, Store, users);
        DecideSellerApplication = new DecideSellerApplicationUseCase(Store, Store, Store, users);
        CreateProduct = new CreateProductUseCase(Store, Store, Store, users);
        UpdateProduct = new UpdateProductUseCase(Store, users, products);
        ChangeProductStatus = new ChangeProductStatusUseCase(Store, users, products);
        PlaceOrder = new PlaceOrderUseCase(Store, Store, Store, Store, users);
        CreateReview = new CreateReviewUseCase(Store, Store, Store);
        OpenConversation = new OpenConversationUseCase(Store, Store, Store, users, products);
        SendMessage = new SendConversationMessageUseCase(Store, Store, conversations);
        GetMessages = new GetConversationMessagesUseCase(Store, conversations);
    }

    internal async Task<Product> PublishedProductAsync(int stock)
    {
        var application = await ApplyAsSeller.ExecuteAsync(
            new ApplyAsSellerCommand(Seller.Id, "Tienda", ""));
        await DecideSellerApplication.ExecuteAsync(
            new DecideSellerApplicationCommand(Admin.Id, application.Id, true));
        var product = await CreateProduct.ExecuteAsync(
            new CreateProductCommand(Seller.Id, ProductData("Vaso", 12, stock)));
        return await ChangeProductStatus.ExecuteAsync(
            new ChangeProductStatusCommand(Seller.Id, product.Id, ProductStatus.Published));
    }

    internal static ProductData ProductData(string name, decimal price, int stock) =>
        new(name, "", "Hogar", price, stock);
}
