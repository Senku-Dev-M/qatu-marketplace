namespace Qatu.Application;

public interface IReportService
{
    Task<AdminReport> GetAsync(Guid adminId);
}
