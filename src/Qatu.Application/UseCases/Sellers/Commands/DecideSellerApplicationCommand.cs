namespace Qatu.Application;

public sealed record DecideSellerApplicationCommand(Guid AdminId, Guid ApplicationId, bool Approve);
