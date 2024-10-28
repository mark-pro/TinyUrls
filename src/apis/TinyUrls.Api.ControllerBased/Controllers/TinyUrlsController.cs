using Microsoft.AspNetCore.Mvc;
using TinyUrls.Api.Controllers.Contracts;
using TinyUrls.Api.Controllers.Services;
using TinyUrls.Types;

namespace TinyUrls.Api.Controllers.Controllers;

[ApiController]
[Route("/")]
public sealed class TinyUrlsController {
    private readonly IShortnerService _shortnerService;

    public TinyUrlsController(
        [FromKeyedServices("cached_shortner")] IShortnerService shortnerService
    ) => (_shortnerService) = (shortnerService);
        
    [HttpGet("{shortCode}")]
    public async ValueTask<IActionResult> RedirectFromShortCode(ShortCodeType shortCode) =>
        await _shortnerService.GetTinyUrl(shortCode) switch {
            (true, { } tiny) => new RedirectResult(tiny.Uri.ToString(), true, true),
            _ => new NotFoundResult()
        };

    [HttpPut]
    public async ValueTask<IActionResult> CreateShortenedUrl([FromBody] CreateRequest request) =>
        await _shortnerService.CreateTinyUrl(request.Uri) switch {
            (true, { } tiny) => new CreatedResult($"/{tiny.ShortCode}", tiny),
            _ => new StatusCodeResult(500)
        };
}