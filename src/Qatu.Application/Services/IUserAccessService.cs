using Qatu.Domain;

namespace Qatu.Application;

public interface IUserAccessService
{
    Task<Profile> EnsureProfileAsync(Guid userId, string email, string? name);
    Task<Profile> GetAsync(Guid userId);
    Task RequireAdminAsync(Guid userId);
    Task RequireSellerAsync(Guid userId);
}
