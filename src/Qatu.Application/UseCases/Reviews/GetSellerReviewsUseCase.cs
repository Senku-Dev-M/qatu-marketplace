using Qatu.Domain;

namespace Qatu.Application;

public sealed class GetSellerReviewsUseCase(IReadRepository reader)
{
    public Task<List<Review>> ExecuteAsync(Guid sellerId) =>
        reader.ListAsync<Review>(review =>
            review.SellerId == sellerId && review.Kind == ReviewKind.Seller);
}
