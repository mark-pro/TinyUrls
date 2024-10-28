using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

namespace TinyUrls.Api.Controllers.Services;

public sealed class DbContextShortnerService : IShortnerService {
    private readonly IShortner _shortner;
    private readonly IOptions<ShortnerConfig> _config;
    private readonly TinyUrlDbContext _context;
    private readonly ILogger<DbContextShortnerService> _logger;

    public DbContextShortnerService(
        IShortner shortner, IOptions<ShortnerConfig> config,
        TinyUrlDbContext context,
        ILogger<DbContextShortnerService> logger
    ) {
        _shortner = shortner;
        _config = config;
        _context = context;
        _logger = logger;
    }
    
    public async ValueTask<(bool success, TinyUrlType? tinyUrl)>  CreateTinyUrl(Uri uri) {
        var tiny = _shortner.Shorten(_config.Value, uri);
        _logger.LogDebug("Inserting [{shortCode}] -> {uri}", tiny.ShortCode, tiny.Uri);
        _context.TinyUrls.Add(tiny);
        try {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to insert [{shortCode}] -> {uri}", tiny.ShortCode, tiny.Uri);
            return (false, null);
        }
        return (true, tiny);
    }
    
    public async ValueTask<(bool success, TinyUrlType? tinyUrl)> GetTinyUrl(ShortCodeType shortCode) {
        try {
            _logger.LogDebug("Retrieving [{shortCode}]", shortCode);
            var tiny = await _context.TinyUrls.FirstOrDefaultAsync(tiny => tiny.ShortCode == shortCode);
            return tiny is null ? (false, null) : (true, tiny);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to retrieve [{shortCode}]", shortCode);
            return (false, null);
        }
    }
}