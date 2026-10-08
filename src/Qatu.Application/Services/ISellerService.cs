using Qatu.Domain;

namespace Qatu.Application;

public interface ISellerService
{
    Task<SellerApplication> ApplyAsync(Guid userId, string shopName, string description);
    Task<List<SellerApplication>> ApplicationsAsync(Guid adminId);
    Task<SellerApplication> DecideAsync(Guid adminId, Guid applicationId, bool approve);
    Task<Shop?> MyShopAsync(Guid sellerId);
}
