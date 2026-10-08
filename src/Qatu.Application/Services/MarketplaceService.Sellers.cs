using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<SellerApplication> ApplySellerAsync(Guid userId, string shopName, string description)
    {
        var user = await UserAsync(userId);
        if (user.IsSeller) throw new MarketException(MarketError.Conflict, "Ya eres vendedor.");
        if (string.IsNullOrWhiteSpace(shopName) || shopName.Trim().Length > 80) throw Invalid("El nombre de la tienda debe tener entre 1 y 80 caracteres.");
        var pending = await store.ListAsync<SellerApplication>(a => a.ApplicantId == userId && a.Status == ApplicationStatus.Pending);
        if (pending.Count > 0) throw new MarketException(MarketError.Conflict, "Ya tienes una solicitud pendiente.");
        var application = new SellerApplication { ApplicantId = userId, ShopName = shopName.Trim(), Description = description.Trim() };
        store.Add(application);
        await store.SaveAsync();
        return application;
    }

    public async Task<List<SellerApplication>> ApplicationsAsync(Guid adminId)
    {
        await RequireAdminAsync(adminId);
        return (await store.ListAsync<SellerApplication>(a => true)).OrderByDescending(a => a.CreatedAt).ToList();
    }

    public async Task<SellerApplication> DecideApplicationAsync(Guid adminId, Guid applicationId, bool approve)
    {
        await RequireAdminAsync(adminId);
        return await store.InTransactionAsync(async () =>
        {
            var application = await store.FindAsync<SellerApplication>(applicationId) ?? throw NotFound();
            if (application.Status != ApplicationStatus.Pending) throw new MarketException(MarketError.Conflict, "La solicitud ya fue resuelta.");
            application.Status = approve ? ApplicationStatus.Approved : ApplicationStatus.Rejected;
            application.DecidedAt = DateTime.UtcNow;
            if (approve)
            {
                var user = await UserAsync(application.ApplicantId);
                user.IsSeller = true;
                store.Add(new Shop { OwnerId = user.Id, Name = application.ShopName, Description = application.Description });
            }
            await store.SaveAsync();
            return application;
        });
    }

    public async Task<Shop?> MyShopAsync(Guid sellerId)
    {
        await RequireSellerAsync(sellerId);
        return (await store.ListAsync<Shop>(s => s.OwnerId == sellerId)).FirstOrDefault();
    }
}
