using Qatu.Domain;

namespace Qatu.Application;

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
            if (command.Approve) await ApproveAsync(application);
            await unitOfWork.SaveAsync();
            return application;
        });
    }

    private async Task ApproveAsync(SellerApplication application)
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
}
