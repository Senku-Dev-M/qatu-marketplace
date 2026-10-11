namespace Qatu.Application;

public sealed record ApplyAsSellerCommand(Guid UserId, string ShopName, string Description);
