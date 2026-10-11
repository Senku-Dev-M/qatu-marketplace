using Microsoft.AspNetCore.Authentication;
using Qatu.Api;
using Qatu.Application;
using Qatu.Infrastructure;

namespace Qatu.Api.Configuration;

internal static class SupabaseRegistration
{
    internal static IServiceCollection AddQatuSupabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var supabaseUrl = configuration["Supabase:Url"] ?? "http://localhost:8000/";
        var publicSupabaseUrl = configuration["Supabase:PublicUrl"] ?? "http://localhost:8000";
        var publishableKey = configuration["Supabase:PublishableKey"] ?? "";
        var serviceRoleKey = configuration["Supabase:ServiceRoleKey"] ?? "";

        services.AddSingleton(new SupabaseStorageOptions(serviceRoleKey));
        services.AddSingleton(new SupabaseAuthOptions(publishableKey));
        services.AddSingleton(new PublicApiOptions(publicSupabaseUrl, publishableKey));
        services.AddHttpClient("supabase", client => client.BaseAddress = BaseAddress(supabaseUrl));
        services.AddHttpClient<IImageStore, SupabaseImageStore>(client =>
            client.BaseAddress = BaseAddress(supabaseUrl));
        services.AddAuthentication("Supabase")
            .AddScheme<AuthenticationSchemeOptions, SupabaseAuthHandler>("Supabase", _ => { });
        services.AddAuthorization();
        return services;
    }

    private static Uri BaseAddress(string url) => new(url.EndsWith('/') ? url : url + "/");
}
