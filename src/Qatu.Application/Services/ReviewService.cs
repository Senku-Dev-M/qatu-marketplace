using Qatu.Domain;

namespace Qatu.Application;

public sealed class ReviewService(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork) : IReviewService
{
    public async Task<Review> CreateAsync(Guid buyerId, Guid orderId, ReviewKind kind, int stars, string comment)
    {
        if (stars is < 1 or > 5 || comment.Length > 500)
            throw MarketErrors.Invalid("La calificación debe tener 1 a 5 estrellas y un comentario de hasta 500 caracteres.");
        var order = await reader.FindAsync<PurchaseOrder>(orderId) ?? throw MarketErrors.NotFound();
        if (order.BuyerId != buyerId || order.Status != OrderStatus.Paid) throw MarketErrors.Forbidden();
        var existing = await reader.ListAsync<Review>(review => review.OrderId == orderId && review.Kind == kind);
        if (existing.Count > 0) throw MarketErrors.Conflict("Ya calificaste esta compra.");

        var review = new Review
        {
            OrderId = orderId,
            BuyerId = buyerId,
            ProductId = order.ProductId,
            SellerId = order.SellerId,
            Kind = kind,
            Stars = stars,
            Comment = comment.Trim()
        };
        writer.Add(review);
        await unitOfWork.SaveAsync();
        return review;
    }

    public Task<List<Review>> ProductReviewsAsync(Guid productId) =>
        reader.ListAsync<Review>(review => review.ProductId == productId && review.Kind == ReviewKind.Product);

    public Task<List<Review>> SellerReviewsAsync(Guid sellerId) =>
        reader.ListAsync<Review>(review => review.SellerId == sellerId && review.Kind == ReviewKind.Seller);
}
