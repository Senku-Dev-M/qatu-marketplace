using Microsoft.EntityFrameworkCore;
using Qatu.Application;
using Qatu.Infrastructure;

namespace Qatu.Api.Configuration;

internal static class PersistenceRegistration
{
    internal static IServiceCollection AddQatuPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Qatu")
            ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<EfAppRepository>();
        services.AddScoped<IReadRepository>(provider => provider.GetRequiredService<EfAppRepository>());
        services.AddScoped<IWriteRepository>(provider => provider.GetRequiredService<EfAppRepository>());
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EfAppRepository>());
        services.AddScoped<IStockRepository>(provider => provider.GetRequiredService<EfAppRepository>());
        return services;
    }
}
