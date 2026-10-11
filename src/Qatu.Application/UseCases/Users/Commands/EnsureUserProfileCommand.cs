namespace Qatu.Application;

public sealed record EnsureUserProfileCommand(Guid UserId, string Email, string? DisplayName);
