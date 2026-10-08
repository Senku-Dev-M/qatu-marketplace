using Qatu.Domain;

namespace Qatu.Application;

public sealed record ApplyAsSellerCommand(Guid UserId, string ShopName, string Description);
public sealed record DecideSellerApplicationCommand(Guid AdminId, Guid ApplicationId, bool Approve);

public sealed class ApplyAsSellerUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    UserAccessPolicy users)
{
    public async Task<SellerApplication> ExecuteAsync(ApplyAsSellerCommand command)
    {
        var user = await users.GetAsync(command.UserId);
        if (user.IsSeller) throw MarketErrors.Conflict("Ya eres vendedor.");
        if (string.IsNullOrWhiteSpace(command.ShopName) || command.ShopName.Trim().Length > 80)
            throw MarketErrors.Invalid("El nombre de la tienda debe tener entre 1 y 80 caracteres.");
        var pending = await reader.ListAsync<SellerApplication>(application =>
            application.ApplicantId == command.UserId && application.Status == ApplicationStatus.Pending);
        if (pending.Count > 0) throw MarketErrors.Conflict("Ya tienes una solicitud pendiente.");

        var application = new SellerApplication
        {
            ApplicantId = command.UserId,
            ShopName = command.ShopName.Trim(),
            Description = command.Description.Trim()
        };
        writer.Add(application);
        await unitOfWork.SaveAsync();
        return application;
    }
}

public sealed class ListSellerApplicationsUseCase(IReadRepository reader, UserAccessPolicy users)
{
    public async Task<List<SellerApplication>> ExecuteAsync(Guid adminId)
    {
        await users.RequireAdminAsync(adminId);
        return (await reader.ListAsync<SellerApplication>(_ => true))
            .OrderByDescending(application => application.CreatedAt)
            .ToList();
    }
}

public sealed class DecideSellerApplicationUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    UserAccessPolicy users)
{
    public async Task<SellerApplication> ExecuteAsync(DecideSellerApplicationCommand command)
    {
        await users.RequireAdminAsync(command.AdminId);
        return await unitOfWork.InTransactionAsync(async () =>
        {
            var application = await reader.FindAsync<SellerApplication>(command.ApplicationId)
                ?? throw MarketErrors.NotFound();
            if (application.Status != ApplicationStatus.Pending)
                throw MarketErrors.Conflict("La solicitud ya fue resuelta.");
            application.Status = command.Approve ? ApplicationStatus.Approved : ApplicationStatus.Rejected;
            application.DecidedAt = DateTime.UtcNow;
            if (command.Approve)
            {
                var user = await users.GetAsync(application.ApplicantId);
                user.IsSeller = true;
                writer.Add(new Shop
                {
                    OwnerId = user.Id,
                    Name = application.ShopName,
                    Description = application.Description
                });
            }
            await unitOfWork.SaveAsync();
            return application;
        });
    }
}

public sealed class GetMyShopUseCase(IReadRepository reader, UserAccessPolicy users)
{
    public async Task<Shop?> ExecuteAsync(Guid sellerId)
    {
        await users.RequireSellerAsync(sellerId);
        return (await reader.ListAsync<Shop>(shop => shop.OwnerId == sellerId)).FirstOrDefault();
    }
}
