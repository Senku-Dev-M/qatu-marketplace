using System.Linq.Expressions;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Tests;

public sealed class MarketplaceTests
{
    private readonly MemoryStore store = new();
    private readonly MarketplaceService market;
    private readonly Profile admin = new() { Email = "admin@test.local", IsAdmin = true };
    private readonly Profile seller = new() { Email = "seller@test.local" };
    private readonly Profile buyer = new() { Email = "buyer@test.local" };

    public MarketplaceTests()
    {
        admin.Id = Guid.NewGuid(); seller.Id = Guid.NewGuid(); buyer.Id = Guid.NewGuid();
        store.Add(admin); store.Add(seller); store.Add(buyer);
        market = new MarketplaceService(store, new NoImages(), new MarketplaceOptions(admin.Id));
    }

    [Fact]
    public async Task SellerRequiresApprovalAndCanOnlyEditOwnProducts()
    {
        var denied = await Assert.ThrowsAsync<MarketException>(() => market.SaveProductAsync(seller.Id, null, "Vaso", "", "Hogar", 10, 1));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
        var application = await market.ApplySellerAsync(seller.Id, "Tienda", "Cerámica");
        await market.DecideApplicationAsync(admin.Id, application.Id, true);
        Assert.True(seller.IsSeller);
        var product = await market.SaveProductAsync(seller.Id, null, "Vaso", "", "Hogar", 10, 2);
        var other = await Assert.ThrowsAsync<MarketException>(() => market.SaveProductAsync(buyer.Id, product.Id, "Otro", "", "Hogar", 10, 1));
        Assert.Equal(MarketError.Forbidden, other.Kind);
    }

    [Fact]
    public async Task FailedPaymentKeepsStockAndCannotBeReviewed()
    {
        var product = await PublishedProductAsync(3);
        var order = await market.BuyAsync(buyer.Id, product.Id, 2, false);
        Assert.Equal(OrderStatus.Failed, order.Status);
        Assert.Equal(3, product.Stock);
        var denied = await Assert.ThrowsAsync<MarketException>(() => market.ReviewAsync(buyer.Id, order.Id, ReviewKind.Product, 5, "Bien"));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
    }

    [Fact]
    public async Task PaidOrderDecrementsStockAndAllowsOneReview()
    {
        var product = await PublishedProductAsync(2);
        var order = await market.BuyAsync(buyer.Id, product.Id, 2, true);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(0, product.Stock);
        var review = await market.ReviewAsync(buyer.Id, order.Id, ReviewKind.Product, 4, "Útil");
        Assert.Equal(4, review.Stars);
        var duplicate = await Assert.ThrowsAsync<MarketException>(() => market.ReviewAsync(buyer.Id, order.Id, ReviewKind.Product, 4, "Otra"));
        Assert.Equal(MarketError.Conflict, duplicate.Kind);
        var noStock = await Assert.ThrowsAsync<MarketException>(() => market.BuyAsync(buyer.Id, product.Id, 1, true));
        Assert.Equal(MarketError.Conflict, noStock.Kind);
    }

    [Fact]
    public async Task ChatHistoryRequiresParticipant()
    {
        var product = await PublishedProductAsync(2);
        var conversation = await market.OpenConversationAsync(buyer.Id, product.Id);
        await market.SendMessageAsync(buyer.Id, conversation.Id, "Hola");
        Assert.Single(await market.MessagesAsync(seller.Id, conversation.Id));
        var denied = await Assert.ThrowsAsync<MarketException>(() => market.MessagesAsync(admin.Id, conversation.Id));
        Assert.Equal(MarketError.Forbidden, denied.Kind);
    }

    private async Task<Product> PublishedProductAsync(int stock)
    {
        var application = await market.ApplySellerAsync(seller.Id, "Tienda", "");
        await market.DecideApplicationAsync(admin.Id, application.Id, true);
        var product = await market.SaveProductAsync(seller.Id, null, "Vaso", "", "Hogar", 12, stock);
        return await market.SetProductStatusAsync(seller.Id, product.Id, ProductStatus.Published);
    }
}

internal sealed class NoImages : IImageStore
{
    public Task<string> UploadAsync(Guid productId, Stream data, string contentType, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<(Stream Data, string ContentType)?> ReadAsync(string path, CancellationToken cancellationToken) => throw new NotImplementedException();
}

internal sealed class MemoryStore : IAppRepository
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
