using Qatu.Domain;

namespace Qatu.Application;

public sealed record CreateReviewCommand(
    Guid BuyerId,
    Guid OrderId,
    ReviewKind Kind,
    int Stars,
    string Comment);

public sealed class CreateReviewUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork)
{
    public async Task<Review> ExecuteAsync(CreateReviewCommand command)
    {
        if (command.Stars is < 1 or > 5 || command.Comment.Length > 500)
            throw MarketErrors.Invalid(
                "La calificación debe tener 1 a 5 estrellas y un comentario de hasta 500 caracteres.");
        var order = await reader.FindAsync<PurchaseOrder>(command.OrderId) ?? throw MarketErrors.NotFound();
        if (order.BuyerId != command.BuyerId || order.Status != OrderStatus.Paid)
            throw MarketErrors.Forbidden();
        var existing = await reader.ListAsync<Review>(review =>
            review.OrderId == command.OrderId && review.Kind == command.Kind);
        if (existing.Count > 0) throw MarketErrors.Conflict("Ya calificaste esta compra.");

        var review = new Review
        {
            OrderId = command.OrderId,
            BuyerId = command.BuyerId,
            ProductId = order.ProductId,
            SellerId = order.SellerId,
            Kind = command.Kind,
            Stars = command.Stars,
            Comment = command.Comment.Trim()
        };
        writer.Add(review);
        await unitOfWork.SaveAsync();
        return review;
    }
}

public sealed class GetSellerReviewsUseCase(IReadRepository reader)
{
    public Task<List<Review>> ExecuteAsync(Guid sellerId) =>
        reader.ListAsync<Review>(review =>
            review.SellerId == sellerId && review.Kind == ReviewKind.Seller);
}
