using Microsoft.AspNetCore.Diagnostics;
using Qatu.Application;

namespace Qatu.Api.Configuration;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseQatuExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
        {
            var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (error is MarketException market)
            {
                context.Response.StatusCode = StatusCodeFor(market.Kind);
                await Results.Problem(market.Message, statusCode: context.Response.StatusCode)
                    .ExecuteAsync(context);
                return;
            }

            app.Logger.LogError(error, "Error inesperado en la API");
            await Results.Problem("Error inesperado.", statusCode: 500).ExecuteAsync(context);
        }));
        return app;
    }

    private static int StatusCodeFor(MarketError error) => error switch
    {
        MarketError.Invalid => StatusCodes.Status400BadRequest,
        MarketError.Forbidden => StatusCodes.Status403Forbidden,
        MarketError.NotFound => StatusCodes.Status404NotFound,
        MarketError.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
