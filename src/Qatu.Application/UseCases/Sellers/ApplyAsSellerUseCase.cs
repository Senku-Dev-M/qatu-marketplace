using Qatu.Domain;

namespace Qatu.Application;

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
