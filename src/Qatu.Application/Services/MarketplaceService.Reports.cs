using Qatu.Domain;

namespace Qatu.Application;

public sealed partial class MarketplaceService
{
    public async Task<AdminReport> ReportAsync(Guid adminId)
    {
        await RequireAdminAsync(adminId);
        var users = await store.ListAsync<Profile>(p => true);
        var products = await store.ListAsync<Product>(p => true);
        var orders = await store.ListAsync<PurchaseOrder>(o => o.Status == OrderStatus.Paid);
        var days = orders.GroupBy(o => DateOnly.FromDateTime(o.PaidAt ?? o.CreatedAt))
            .OrderBy(g => g.Key)
            .Select(g => new DailySale(g.Key, g.Count(), g.Sum(o => o.UnitPriceBob * o.Quantity))).ToList();
        return new AdminReport(users.Count, users.Count(u => u.IsSeller), products.Count, orders.Count,
            orders.Sum(o => o.UnitPriceBob * o.Quantity), days);
    }
}
