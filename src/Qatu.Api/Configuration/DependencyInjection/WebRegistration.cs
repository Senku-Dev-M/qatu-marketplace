using System.Text.Json.Serialization;

namespace Qatu.Api.Configuration;

internal static class WebRegistration
{
    internal static IServiceCollection AddQatuWebServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddProblemDetails();
        services.AddSignalR();
        services.AddCors(options => options.AddDefaultPolicy(policy => policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:8080",
                "http://localhost:8081",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:8080",
                "http://127.0.0.1:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));
        return services;
    }
}
