using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

namespace TinyUrls.Api.Minimal;

public static class Endpoints {
    public static WebApplication MapEndpoints(this WebApplication app) {
        app.MapGet("/{shortCode}", RedirectFromShortCode);
        app.MapPut("/", CreateShortenedUrl);

        return app;
    }

    internal static IResult RedirectFromShortCode(
        [FromRoute] ShortCodeType shortCode, TinyUrlDbContext context,
        ILogger<WebApplication> logger) =>
        context.TinyUrls.FirstOrOptional(url => url.ShortCode == shortCode)
            .Do(tiny => logger.LogDebug("Redirecting to {uri}", tiny.Uri))
            .Map(tiny => Results.Redirect(tiny.Uri.ToString(), true, true))
            .IfNone(Results.NotFound());
    
    internal static IResult CreateShortenedUrl(
        [FromBody] CreateRequest request, IOptions<ShortnerConfig> config,
        IShortner shortner, TinyUrlDbContext context
    ) {
        var tiny = shortner.Shorten(config.Value, request.Uri);
        context.TinyUrls.Add(tiny);
        context.SaveChanges();
        return Results.Created($"/{tiny.ShortCode}", tiny);
    }
}