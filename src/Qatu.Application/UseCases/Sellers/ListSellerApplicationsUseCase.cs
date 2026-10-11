using Qatu.Domain;

namespace Qatu.Application;

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
