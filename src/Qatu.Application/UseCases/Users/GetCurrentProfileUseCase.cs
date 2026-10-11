using Qatu.Domain;

namespace Qatu.Application;

public sealed class GetCurrentProfileUseCase(UserAccessPolicy users)
{
    public Task<Profile> ExecuteAsync(Guid userId) => users.GetAsync(userId);
}
