using Qatu.Domain;

namespace Qatu.Application;

public sealed class SellerService(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    IUserAccessService users) : ISellerService
{
    public async Task<SellerApplication> ApplyAsync(Guid userId, string shopName, string description)
    {
        var user = await users.GetAsync(userId);
        if (user.IsSeller) throw MarketErrors.Conflict("Ya eres vendedor.");
        if (string.IsNullOrWhiteSpace(shopName) || shopName.Trim().Length > 80)
            throw MarketErrors.Invalid("El nombre de la tienda debe tener entre 1 y 80 caracteres.");
        var pending = await reader.ListAsync<SellerApplication>(a => a.ApplicantId == userId && a.Status == ApplicationStatus.Pending);
        if (pending.Count > 0) throw MarketErrors.Conflict("Ya tienes una solicitud pendiente.");

        var application = new SellerApplication { ApplicantId = userId, ShopName = shopName.Trim(), Description = description.Trim() };
        writer.Add(application);
        await unitOfWork.SaveAsync();
        return application;
    }

    public async Task<List<SellerApplication>> ApplicationsAsync(Guid adminId)
    {
        await users.RequireAdminAsync(adminId);
        return (await reader.ListAsync<SellerApplication>(a => true)).OrderByDescending(a => a.CreatedAt).ToList();
    }

    public async Task<SellerApplication> DecideAsync(Guid adminId, Guid applicationId, bool approve)
    {
        await users.RequireAdminAsync(adminId);
        return await unitOfWork.InTransactionAsync(async () =>
        {
            var application = await reader.FindAsync<SellerApplication>(applicationId) ?? throw MarketErrors.NotFound();
            if (application.Status != ApplicationStatus.Pending) throw MarketErrors.Conflict("La solicitud ya fue resuelta.");
            application.Status = approve ? ApplicationStatus.Approved : ApplicationStatus.Rejected;
            application.DecidedAt = DateTime.UtcNow;
            if (approve)
            {
                var user = await users.GetAsync(application.ApplicantId);
                user.IsSeller = true;
                writer.Add(new Shop { OwnerId = user.Id, Name = application.ShopName, Description = application.Description });
            }
            await unitOfWork.SaveAsync();
            return application;
        });
    }

    public async Task<Shop?> MyShopAsync(Guid sellerId)
    {
        await users.RequireSellerAsync(sellerId);
        return (await reader.ListAsync<Shop>(shop => shop.OwnerId == sellerId)).FirstOrDefault();
    }
}
