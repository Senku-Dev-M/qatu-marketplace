using Qatu.Domain;

namespace Qatu.Application;

public sealed class UserAccessService(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    MarketplaceOptions options) : IUserAccessService
{
    public async Task<Profile> EnsureProfileAsync(Guid userId, string email, string? name)
    {
        var profile = await reader.FindAsync<Profile>(userId);
        if (profile is not null) return profile;

        profile = new Profile
        {
            Id = userId,
            Email = email.Trim(),
            DisplayName = string.IsNullOrWhiteSpace(name) ? email.Split('@')[0] : name.Trim(),
            IsAdmin = options.AdminUserId == userId
        };
        writer.Add(profile);
        await unitOfWork.SaveAsync();
        return profile;
    }

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
