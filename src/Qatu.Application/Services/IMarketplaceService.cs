using Qatu.Domain;

namespace Qatu.Application;

public interface IMarketplaceService
{
    Task<Profile> EnsureProfileAsync(Guid userId, string email, string? name);

    Task<List<ProductCard>> CatalogAsync(string? query, string? category, Guid? sellerId = null, bool includeDrafts = false);
    Task<Product> ProductAsync(Guid id, Guid? viewerId = null);
    Task<(Stream Data, string ContentType)?> ProductImageAsync(Guid productId, CancellationToken cancellationToken);

    Task<SellerApplication> ApplySellerAsync(Guid userId, string shopName, string description);
    Task<List<SellerApplication>> ApplicationsAsync(Guid adminId);
    Task<SellerApplication> DecideApplicationAsync(Guid adminId, Guid applicationId, bool approve);
    Task<Shop?> MyShopAsync(Guid sellerId);

    Task<Product> SaveProductAsync(Guid sellerId, Guid? productId, string name, string description,
        string category, decimal priceBob, int stock, string? brand = null, string? presentation = null, string? unit = null);
    Task<Product> SetProductStatusAsync(Guid sellerId, Guid productId, ProductStatus status);
    Task<Product> UploadProductImageAsync(Guid sellerId, Guid productId, Stream data, string contentType, CancellationToken cancellationToken);

    Task<PurchaseOrder> BuyAsync(Guid buyerId, Guid productId, int quantity, bool paymentSucceeds);
    Task<List<PurchaseOrder>> OrdersAsync(Guid userId);

    Task<Conversation> OpenConversationAsync(Guid buyerId, Guid productId);
    Task<List<Conversation>> ConversationsAsync(Guid userId);
    Task<Conversation> ConversationAsync(Guid userId, Guid id);
    Task<List<ChatMessage>> MessagesAsync(Guid userId, Guid conversationId);
    Task<ChatMessage> SendMessageAsync(Guid userId, Guid conversationId, string text);

    Task<Review> ReviewAsync(Guid buyerId, Guid orderId, ReviewKind kind, int stars, string comment);
    Task<List<Review>> ProductReviewsAsync(Guid productId);
    Task<List<Review>> SellerReviewsAsync(Guid sellerId);

    Task<AdminReport> ReportAsync(Guid adminId);
}
