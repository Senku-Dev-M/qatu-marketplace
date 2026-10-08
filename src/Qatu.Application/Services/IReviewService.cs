using Qatu.Domain;

namespace Qatu.Application;

public interface IReviewService
{
    Task<Review> CreateAsync(Guid buyerId, Guid orderId, ReviewKind kind, int stars, string comment);
    Task<List<Review>> ProductReviewsAsync(Guid productId);
    Task<List<Review>> SellerReviewsAsync(Guid sellerId);
}
