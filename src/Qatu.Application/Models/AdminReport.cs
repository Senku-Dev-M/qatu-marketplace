namespace Qatu.Application;

public sealed record AdminReport(int Users, int Sellers, int Products, int PaidOrders,
    decimal SalesBob, IReadOnlyList<DailySale> DailySales);
