namespace Qatu.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQatuServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddQatuWebServices();
        services.AddQatuPersistence(configuration);
        services.AddQatuApplication(configuration);
        services.AddQatuSupabase(configuration);
        return services;
    }
}
