using System.Security.Claims;
using Qatu.Api.Serialization;
using Qatu.Application;

namespace Qatu.Api.Endpoints;

public static class ReportsEndpoints
{
    public static void MapReports(this RouteGroupBuilder secured)
    {
        secured.MapGet("/admin/report", (ClaimsPrincipal user, GetAdminReportUseCase useCase) =>
            useCase.ExecuteAsync(user.Id()));

        secured.MapGet("/admin/report.csv", async (ClaimsPrincipal user, GetAdminReportUseCase useCase) =>
        {
            var report = await useCase.ExecuteAsync(user.Id());
            return Results.File(
                AdminReportCsvWriter.Write(report),
                "text/csv; charset=utf-8",
                "qatu-ventas.csv");
        });
    }
}
