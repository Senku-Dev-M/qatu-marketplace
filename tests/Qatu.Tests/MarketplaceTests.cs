using System.Linq.Expressions;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Tests;

public sealed class MarketplaceTests
{
    private readonly MemoryStore store = new();
    private readonly ApplyAsSellerUseCase applyAsSeller;
    private readonly DecideSellerApplicationUseCase decideSellerApplication;
    private readonly CreateProductUseCase createProduct;
    private readonly UpdateProductUseCase updateProduct;
    private readonly ChangeProductStatusUseCase changeProductStatus;
    private readonly PlaceOrderUseCase placeOrder;
    private readonly CreateReviewUseCase createReview;
    private readonly OpenConversationUseCase openConversation;
    private readonly SendConversationMessageUseCase sendMessage;
    private readonly GetConversationMessagesUseCase getMessages;
    private readonly Profile admin = new() { Email = "admin@test.local", IsAdmin = true };
    private readonly Profile seller = new() { Email = "seller@test.local" };
    private readonly Profile buyer = new() { Email = "buyer@test.local" };

    public MarketplaceTests()
    {
        admin.Id = Guid.NewGuid(); seller.Id = Guid.NewGuid(); buyer.Id = Guid.NewGuid();
        store.Add(admin); store.Add(seller); store.Add(buyer);
        var users = new UserAccessPolicy(store);
        var products = new ProductAccessPolicy(store);
        var conversations = new ConversationAccessPolicy(store);
        applyAsSeller = new ApplyAsSellerUseCase(store, store, store, users);
        decideSellerApplication = new DecideSellerApplicationUseCase(store, store, store, users);
        createProduct = new CreateProductUseCase(store, store, store, users);
        updateProduct = new UpdateProductUseCase(store, users, products);
        changeProductStatus = new ChangeProductStatusUseCase(store, users, products);
        placeOrder = new PlaceOrderUseCase(store, store, store, store, users);
        createReview = new CreateReviewUseCase(store, store, store);
        openConversation = new OpenConversationUseCase(store, store, store, users, products);
        sendMessage = new SendConversationMessageUseCase(store, store, conversations);
        getMessages = new GetConversationMessagesUseCase(store, conversations);
    }

    [Fact]
    public async Task SellerRequiresApprovalAndCanOnlyEditOwnProducts()
    {
        var denied = await Assert.ThrowsAsync<MarketException>(() => createProduct.ExecuteAsync(
            new CreateProductCommand(seller.Id, Product("Vaso", 10, 1))));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
        var application = await applyAsSeller.ExecuteAsync(new ApplyAsSellerCommand(seller.Id, "Tienda", "Cerámica"));
        await decideSellerApplication.ExecuteAsync(new DecideSellerApplicationCommand(admin.Id, application.Id, true));
        Assert.True(seller.IsSeller);
        var product = await createProduct.ExecuteAsync(new CreateProductCommand(seller.Id, Product("Vaso", 10, 2)));
        var other = await Assert.ThrowsAsync<MarketException>(() => updateProduct.ExecuteAsync(
            new UpdateProductCommand(buyer.Id, product.Id, Product("Otro", 10, 1))));
        Assert.Equal(MarketError.Forbidden, other.Kind);
    }

    [Fact]
    public async Task FailedPaymentKeepsStockAndCannotBeReviewed()
    {
        var product = await PublishedProductAsync(3);
        var order = await placeOrder.ExecuteAsync(new PlaceOrderCommand(buyer.Id, product.Id, 2, false));
        Assert.Equal(OrderStatus.Failed, order.Status);
        Assert.Equal(3, product.Stock);
        var denied = await Assert.ThrowsAsync<MarketException>(() => createReview.ExecuteAsync(
            new CreateReviewCommand(buyer.Id, order.Id, ReviewKind.Product, 5, "Bien")));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
    }

    [Fact]
    public async Task PaidOrderDecrementsStockAndAllowsOneReview()
    {
        var product = await PublishedProductAsync(2);
        var order = await placeOrder.ExecuteAsync(new PlaceOrderCommand(buyer.Id, product.Id, 2, true));
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(0, product.Stock);
        var review = await createReview.ExecuteAsync(
            new CreateReviewCommand(buyer.Id, order.Id, ReviewKind.Product, 4, "Útil"));
        Assert.Equal(4, review.Stars);
        var duplicate = await Assert.ThrowsAsync<MarketException>(() => createReview.ExecuteAsync(
            new CreateReviewCommand(buyer.Id, order.Id, ReviewKind.Product, 4, "Otra")));
        Assert.Equal(MarketError.Conflict, duplicate.Kind);
        var noStock = await Assert.ThrowsAsync<MarketException>(() => placeOrder.ExecuteAsync(
            new PlaceOrderCommand(buyer.Id, product.Id, 1, true)));
        Assert.Equal(MarketError.Conflict, noStock.Kind);
    }

    [Fact]
    public async Task ChatHistoryRequiresParticipant()
    {
        var product = await PublishedProductAsync(2);
        var conversation = await openConversation.ExecuteAsync(new OpenConversationCommand(buyer.Id, product.Id));
        await sendMessage.ExecuteAsync(new SendConversationMessageCommand(buyer.Id, conversation.Id, "Hola"));
        Assert.Single(await getMessages.ExecuteAsync(new GetConversationMessagesQuery(seller.Id, conversation.Id)));
        var denied = await Assert.ThrowsAsync<MarketException>(() => getMessages.ExecuteAsync(
            new GetConversationMessagesQuery(admin.Id, conversation.Id)));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
    }

    private async Task<Product> PublishedProductAsync(int stock)
    {
        var application = await applyAsSeller.ExecuteAsync(new ApplyAsSellerCommand(seller.Id, "Tienda", ""));
        await decideSellerApplication.ExecuteAsync(new DecideSellerApplicationCommand(admin.Id, application.Id, true));
        var product = await createProduct.ExecuteAsync(new CreateProductCommand(seller.Id, Product("Vaso", 12, stock)));
        return await changeProductStatus.ExecuteAsync(
            new ChangeProductStatusCommand(seller.Id, product.Id, ProductStatus.Published));
    }

    private static ProductData Product(string name, decimal price, int stock) =>
        new(name, "", "Hogar", price, stock);
}

internal sealed class MemoryStore : IReadRepository, IWriteRepository, IUnitOfWork, IStockRepository
{
    private readonly Dictionary<Type, List<object>> data = new();
    private List<object> Items<T>() => data.TryGetValue(typeof(T), out var items) ? items : data[typeof(T)] = [];
    public Task<T?> FindAsync<T>(Guid id) where T : class => Task.FromResult(Items<T>().Cast<T>().FirstOrDefault(x => (Guid)typeof(T).GetProperty("Id")!.GetValue(x)! == id));
    public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> predicate) where T : class => Task.FromResult(Items<T>().Cast<T>().Where(predicate.Compile()).ToList());
    public void Add<T>(T entity) where T : class => Items<T>().Add(entity);
    public Task SaveAsync() => Task.CompletedTask;
    public Task<TResult> InTransactionAsync<TResult>(Func<Task<TResult>> operation) => operation();
    public async Task<bool> TryDecrementStockAsync(Guid id, int quantity)
    {
        var product = await FindAsync<Product>(id);
        if (product is null || product.Stock < quantity) return false;
        product.Stock -= quantity;
        return true;
    }
}
