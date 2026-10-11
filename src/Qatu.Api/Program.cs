using Qatu.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddQatuServices(builder.Configuration);

var app = builder.Build();
app.UseQatuExceptionHandling();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapQatuEndpoints();
await app.MigrateQatuDatabaseAsync();
await app.RunAsync();

public partial class Program { }
