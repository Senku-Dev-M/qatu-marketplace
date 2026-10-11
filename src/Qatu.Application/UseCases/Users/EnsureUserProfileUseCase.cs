using Qatu.Domain;

namespace Qatu.Application;

public sealed class EnsureUserProfileUseCase(
    IReadRepository reader,
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    MarketplaceOptions options)
{
    public async Task<Profile> ExecuteAsync(EnsureUserProfileCommand command)
    {
        var profile = await reader.FindAsync<Profile>(command.UserId);
        if (profile is not null) return profile;

        profile = new Profile
        {
            Id = command.UserId,
            Email = command.Email.Trim(),
            DisplayName = string.IsNullOrWhiteSpace(command.DisplayName)
                ? command.Email.Split('@')[0]
                : command.DisplayName.Trim(),
            IsAdmin = options.AdminUserId == command.UserId
        };
        writer.Add(profile);
        await unitOfWork.SaveAsync();
        return profile;
    }
}
