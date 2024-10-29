using Microsoft.Extensions.Options;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

namespace TinyUrls.Api.Controllers.Services;

public sealed class DbContextShortnerService : IShortnerService {
    private readonly IShortner _shortner;
    private readonly IOptions<ShortnerConfig> _config;
    private readonly IDbContext<TinyUrlDbContext> _context;
    private readonly ILogger<DbContextShortnerService> _logger;
    private readonly CancellationToken _cancellationToken;
    
    public DbContextShortnerService(
        IShortner shortner, IOptions<ShortnerConfig> config,
        IDbContext<TinyUrlDbContext> context,
        ILogger<DbContextShortnerService> logger,
        CancellationToken cancellationToken = default
    ) {
        _shortner = shortner;
        _config = config;
        _context = context;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }
    
    public async ValueTask<(bool success, TinyUrlType? tinyUrl)>  CreateTinyUrl(Uri uri) {
        var tiny = _shortner.Shorten(_config.Value, uri);
        _logger.LogDebug("Inserting [{shortCode}] -> {uri}", tiny.ShortCode, tiny.Uri);
        _context.Set<TinyUrlType>().Add(tiny);
        try {
            await _context.SaveChangesAsync(_cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to insert [{shortCode}] -> {uri}", tiny.ShortCode, tiny.Uri);
            return (false, null);
        }
        return (true, tiny);
    }
    
    public ValueTask<(bool success, TinyUrlType? tinyUrl)> GetTinyUrl(ShortCodeType shortCode) {
        try {
            _logger.LogDebug("Retrieving [{shortCode}]", shortCode);
            var tiny = _context.Set<TinyUrlType>()
                .FirstOrDefault(tiny => tiny.ShortCode == shortCode);
            return ValueTask.FromResult((tiny is {}, tiny));
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to retrieve [{shortCode}]", shortCode);
            return ValueTask.FromResult<(bool, TinyUrlType?)>((false, null));
        }
    }
}