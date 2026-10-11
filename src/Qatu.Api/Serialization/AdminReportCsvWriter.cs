using System.Globalization;
using System.Text;
using Qatu.Application;

namespace Qatu.Api.Serialization;

public static class AdminReportCsvWriter
{
    public static byte[] Write(AdminReport report)
    {
        var csv = new StringBuilder("fecha,pedidos,ventas_bob\n");
        foreach (var day in report.DailySales)
            csv.AppendLine(
                $"{day.Day:yyyy-MM-dd},{day.Orders},{day.SalesBob.ToString(CultureInfo.InvariantCulture)}");
        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}
