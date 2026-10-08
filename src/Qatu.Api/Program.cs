using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Qatu.Api;
using Qatu.Api.Endpoints;
using Qatu.Application;
using Qatu.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "http://localhost:8000/";
var publicSupabaseUrl = builder.Configuration["Supabase:PublicUrl"] ?? "http://localhost:8000";
var publishableKey = builder.Configuration["Supabase:PublishableKey"] ?? "";
var serviceRoleKey = builder.Configuration["Supabase:ServiceRoleKey"] ?? "";
var connectionString = builder.Configuration.GetConnectionString("Qatu") ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";
Guid? adminUserId = Guid.TryParse(builder.Configuration["Bootstrap:AdminUserId"], out var adminId) ? adminId : null;

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<EfAppRepository>();
builder.Services.AddScoped<IReadRepository>(services => services.GetRequiredService<EfAppRepository>());
builder.Services.AddScoped<IWriteRepository>(services => services.GetRequiredService<EfAppRepository>());
builder.Services.AddScoped<IUnitOfWork>(services => services.GetRequiredService<EfAppRepository>());
builder.Services.AddScoped<IStockRepository>(services => services.GetRequiredService<EfAppRepository>());
builder.Services.AddScoped<UserAccessPolicy>();
builder.Services.AddScoped<ProductAccessPolicy>();
builder.Services.AddScoped<ConversationAccessPolicy>();
builder.Services.AddScoped<EnsureUserProfileUseCase>();
builder.Services.AddScoped<GetCurrentProfileUseCase>();
builder.Services.AddScoped<GetCatalogUseCase>();
builder.Services.AddScoped<GetProductDetailsUseCase>();
builder.Services.AddScoped<GetProductImageUseCase>();
builder.Services.AddScoped<GetSellerProductsUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<ChangeProductStatusUseCase>();
builder.Services.AddScoped<UploadProductImageUseCase>();
builder.Services.AddScoped<ApplyAsSellerUseCase>();
builder.Services.AddScoped<ListSellerApplicationsUseCase>();
builder.Services.AddScoped<DecideSellerApplicationUseCase>();
builder.Services.AddScoped<GetMyShopUseCase>();
builder.Services.AddScoped<PlaceOrderUseCase>();
builder.Services.AddScoped<GetUserOrdersUseCase>();
builder.Services.AddScoped<CreateReviewUseCase>();
builder.Services.AddScoped<GetSellerReviewsUseCase>();
builder.Services.AddScoped<OpenConversationUseCase>();
builder.Services.AddScoped<ListConversationsUseCase>();
builder.Services.AddScoped<AuthorizeConversationUseCase>();
builder.Services.AddScoped<GetConversationMessagesUseCase>();
builder.Services.AddScoped<SendConversationMessageUseCase>();
builder.Services.AddScoped<GetAdminReportUseCase>();
builder.Services.AddSingleton(new MarketplaceOptions(adminUserId));
builder.Services.AddSingleton(new SupabaseStorageOptions(serviceRoleKey));
builder.Services.AddHttpClient("supabase", client => client.BaseAddress = new Uri(supabaseUrl.EndsWith('/') ? supabaseUrl : supabaseUrl + "/"));
builder.Services.AddHttpClient<IImageStore, SupabaseImageStore>(client => client.BaseAddress = new Uri(supabaseUrl.EndsWith('/') ? supabaseUrl : supabaseUrl + "/"));
builder.Services.AddSingleton(new SupabaseAuthOptions(publishableKey));
builder.Services.AddAuthentication("Supabase").AddScheme<AuthenticationSchemeOptions, SupabaseAuthHandler>("Supabase", _ => { });
builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins("http://localhost:5173", "http://localhost:8080", "http://localhost:8081", "http://127.0.0.1:5173", "http://127.0.0.1:8080", "http://127.0.0.1:8081")
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (error is MarketException market)
    {
        context.Response.StatusCode = market.Kind switch
        {
            MarketError.Invalid => 400,
            MarketError.Forbidden => 403,
            MarketError.NotFound => 404,
            MarketError.Conflict => 409,
            _ => 500
        };
        await Results.Problem(market.Message, statusCode: context.Response.StatusCode).ExecuteAsync(context);
    }
    else
    {
        app.Logger.LogError(error, "Error inesperado en la API");
        await Results.Problem("Error inesperado.", statusCode: 500).ExecuteAsync(context);
    }
}));
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapGet("/health", async (AppDbContext db) => await db.Database.CanConnectAsync()
    ? Results.Ok(new { status = "healthy" }) : Results.StatusCode(503));

var api = app.MapGroup("/api");
api.MapGet("/config", () => Results.Ok(new { supabaseUrl = publicSupabaseUrl, publishableKey }));
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

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
await app.RunAsync();

public partial class Program { }
