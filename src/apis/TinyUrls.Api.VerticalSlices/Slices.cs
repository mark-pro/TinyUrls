using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

namespace TinyUrls.Api.VerticalSlices;

public record struct CreateTinyUrlRequest(Uri Uri) : IRequest<TinyUrlType?>;

public sealed class CreateTinyUrlNotification(
    IDbContext<TinyUrlDbContext> context, IShortner shortner, IOptions<ShortnerConfig> config) 
    : IRequestHandler<CreateTinyUrlRequest, TinyUrlType?> {
    public async Task<TinyUrlType?> Handle(CreateTinyUrlRequest request, CancellationToken cancellationToken) {
        var tinyUrl = shortner.Shorten(config.Value, request.Uri);
        
        try {
            await context.Set<TinyUrlType>().AddAsync(tinyUrl, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch {
            return null;
        }

        return tinyUrl;
    }
}

public record struct RetrieveTinyUrlRequest(ShortCodeType ShortCode) : IRequest<TinyUrlType?>;

public sealed class RetrieveTinyUrlNotification(IDbContext<TinyUrlDbContext> context) 
    : IRequestHandler<RetrieveTinyUrlRequest, TinyUrlType?> {
    public async Task<TinyUrlType?> Handle(RetrieveTinyUrlRequest request, CancellationToken cancellationToken) {
        try {
            return await context.Set<TinyUrlType>()
                .FirstOrDefaultAsync(tiny => tiny.ShortCode == request.ShortCode, cancellationToken);
        }
        catch {
            return null;
        }
    }
}