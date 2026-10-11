using Microsoft.EntityFrameworkCore;
using Qatu.Api.Endpoints;
using Qatu.Infrastructure;

namespace Qatu.Api.Configuration;

public static class EndpointMappingExtensions
{
    public static WebApplication MapQatuEndpoints(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapGet("/health", async (AppDbContext db) => await db.Database.CanConnectAsync()
            ? Results.Ok(new { status = "healthy" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable));
        app.MapGet("/api/config", (PublicApiOptions options) => Results.Ok(new
        {
            supabaseUrl = options.SupabaseUrl,
            publishableKey = options.PublishableKey
        }));

        var api = app.MapGroup("/api");
        var secured = api.MapGroup("").RequireAuthorization();
        api.MapCatalog();
        api.MapReviews(secured);
        secured.MapProfiles();
        secured.MapSellers();
        secured.MapProducts();
        secured.MapOrders();
        secured.MapConversations();
        secured.MapReports();
        app.MapHub<ChatHub>("/hubs/chat").RequireAuthorization();
        return app;
    }

    public static async Task MigrateQatuDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}
