using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<Review> ReviewAsync(Guid buyerId, Guid orderId, ReviewKind kind, int stars, string comment)
    {
        if (stars is < 1 or > 5 || comment.Length > 500) throw Invalid("La calificación debe tener 1 a 5 estrellas y un comentario de hasta 500 caracteres.");
        var order = await store.FindAsync<PurchaseOrder>(orderId) ?? throw NotFound();
        if (order.BuyerId != buyerId || order.Status != OrderStatus.Paid) throw Forbidden();
        if ((await store.ListAsync<Review>(r => r.OrderId == orderId && r.Kind == kind)).Count > 0)
            throw new MarketException(MarketError.Conflict, "Ya calificaste esta compra.");
        var review = new Review { OrderId = orderId, BuyerId = buyerId, ProductId = order.ProductId, SellerId = order.SellerId, Kind = kind, Stars = stars, Comment = comment.Trim() };
        store.Add(review);
        await store.SaveAsync();
        return review;
    }

    public Task<List<Review>> ProductReviewsAsync(Guid productId) => store.ListAsync<Review>(r => r.ProductId == productId && r.Kind == ReviewKind.Product);

    public Task<List<Review>> SellerReviewsAsync(Guid sellerId) => store.ListAsync<Review>(r => r.SellerId == sellerId && r.Kind == ReviewKind.Seller);
}
