using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Qatu.Application;

namespace Qatu.Api;

public sealed class SupabaseAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IHttpClientFactory clients,
    SupabaseAuthOptions supabase,
    EnsureUserProfileUseCase ensureUserProfile) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..] : null;
        if (token is null && Request.Path.StartsWithSegments("/hubs/chat")) token = Request.Query["access_token"];
        if (string.IsNullOrWhiteSpace(token)) return AuthenticateResult.NoResult();
        if (string.IsNullOrWhiteSpace(supabase.PublishableKey)) return AuthenticateResult.Fail("Supabase no está configurado.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "auth/v1/user");
            request.Headers.Add("apikey", supabase.PublishableKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await clients.CreateClient("supabase").SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode) return AuthenticateResult.Fail("Token inválido.");
            using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(Context.RequestAborted), cancellationToken: Context.RequestAborted);
            var root = document.RootElement;
            var id = root.GetProperty("id").GetGuid();
            var email = root.TryGetProperty("email", out var emailValue) ? emailValue.GetString() ?? "" : "";
            string? name = null;
            if (root.TryGetProperty("user_metadata", out var metadata) && metadata.ValueKind == JsonValueKind.Object && metadata.TryGetProperty("full_name", out var fullName))
                name = fullName.GetString();
            await ensureUserProfile.ExecuteAsync(new EnsureUserProfileCommand(id, email, name));
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()), new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Name, name ?? email) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        catch (Exception error) when (error is HttpRequestException or JsonException)
        {
            Logger.LogWarning(error, "No se pudo validar la sesión con Supabase");
            return AuthenticateResult.Fail("No se pudo validar la sesión.");
        }
    }
}
