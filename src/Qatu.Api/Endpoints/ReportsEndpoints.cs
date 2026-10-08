using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Qatu.Api.Contracts;
using Qatu.Application;
using Qatu.Domain;

namespace Qatu.Api.Endpoints;

public static class ReportsEndpoints
{
    public static void MapReports(this RouteGroupBuilder secured)
    {
        secured.MapGet("/admin/report", (ClaimsPrincipal user, IReportService reports) => reports.GetAsync(user.Id()));

        secured.MapGet("/admin/report.csv", async (ClaimsPrincipal user, IReportService reports) =>
        {
            var report = await reports.GetAsync(user.Id());
            var csv = new StringBuilder("fecha,pedidos,ventas_bob\n");
            foreach (var day in report.DailySales) csv.AppendLine($"{day.Day:yyyy-MM-dd},{day.Orders},{day.SalesBob.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            return Results.File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv; charset=utf-8", "qatu-ventas.csv");
        });
    }
}
