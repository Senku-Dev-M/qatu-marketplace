using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<Profile> EnsureProfileAsync(Guid userId, string email, string? name)
    {
        var profile = await store.FindAsync<Profile>(userId);
        if (profile is null)
        {
            profile = new Profile { Id = userId, Email = email.Trim(), DisplayName = string.IsNullOrWhiteSpace(name) ? email.Split('@')[0] : name.Trim(), IsAdmin = options.AdminUserId == userId };
            store.Add(profile);
            await store.SaveAsync();
        }
        return profile;
    }

    private async Task<Profile> UserAsync(Guid id) => await store.FindAsync<Profile>(id) ?? throw Forbidden();
    private async Task RequireAdminAsync(Guid id) { if (!(await UserAsync(id)).IsAdmin) throw Forbidden(); }
    private async Task RequireSellerAsync(Guid id) { if (!(await UserAsync(id)).IsSeller) throw Forbidden(); }
}
