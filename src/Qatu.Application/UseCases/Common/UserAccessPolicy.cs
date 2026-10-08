using Qatu.Domain;

namespace Qatu.Application;

public sealed class UserAccessPolicy(IReadRepository reader)
{
    public async Task<Profile> GetAsync(Guid userId) =>
        await reader.FindAsync<Profile>(userId) ?? throw MarketErrors.Forbidden();

    public async Task RequireAdminAsync(Guid userId)
    {
        if (!(await GetAsync(userId)).IsAdmin) throw MarketErrors.Forbidden();
    }

    public async Task RequireSellerAsync(Guid userId)
    {
        if (!(await GetAsync(userId)).IsSeller) throw MarketErrors.Forbidden();
    }
}
