using Qatu.Domain;

namespace Qatu.Application;

public sealed class ReportService(IReadRepository reader, IUserAccessService users) : IReportService
{
    public async Task<AdminReport> GetAsync(Guid adminId)
    {
        await users.RequireAdminAsync(adminId);
        var profiles = await reader.ListAsync<Profile>(profile => true);
        var products = await reader.ListAsync<Product>(product => true);
        var orders = await reader.ListAsync<PurchaseOrder>(order => order.Status == OrderStatus.Paid);
        var dailySales = orders.GroupBy(order => DateOnly.FromDateTime(order.PaidAt ?? order.CreatedAt))
            .OrderBy(group => group.Key)
            .Select(group => new DailySale(group.Key, group.Count(), group.Sum(order => order.UnitPriceBob * order.Quantity)))
            .ToList();
        return new AdminReport(
            profiles.Count,
            profiles.Count(profile => profile.IsSeller),
            products.Count,
            orders.Count,
            orders.Sum(order => order.UnitPriceBob * order.Quantity),
            dailySales);
    }
}
