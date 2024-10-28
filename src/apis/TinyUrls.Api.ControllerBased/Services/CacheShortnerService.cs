using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TinyUrls.Types;

namespace TinyUrls.Api.Controllers.Services;

public sealed class CacheShortnerService : IShortnerService {
    private readonly IOptions<DistributedCacheEntryOptions> _cacheConfig;
    private readonly IDistributedCache _cache;
    private readonly IShortnerService _inner;
    private readonly ILogger<CacheShortnerService> _logger;
    
    public CacheShortnerService(
        IOptions<DistributedCacheEntryOptions> cacheConfig,
        IDistributedCache cache,
        IShortnerService inner,
        ILogger<CacheShortnerService> logger
    ) {
        _cacheConfig = cacheConfig;
        _cache = cache;
        _inner = inner;
        _logger = logger;
    }

    public async ValueTask<(bool success, TinyUrlType? tinyUrl)> CreateTinyUrl(Uri uri) {
        try {
            var result = await _inner.CreateTinyUrl(uri);
            if (result is (false, _)) return result;
            if (result is (true, { } tiny))
                await _cache.SetStringAsync(tiny.ShortCode, tiny.Uri.ToString(), _cacheConfig.Value);
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to create tiny url for {uri}", uri);
            return (false, null);
        }
    }

    public async ValueTask<(bool success, TinyUrlType? tinyUrl)> GetTinyUrl(ShortCodeType shortCode) {
        try {
            var result = await _cache.GetStringAsync(shortCode);
            if (result is { } _) return (true, TinyUrl.Create(shortCode, new Uri(result)));

            var (success, tiny) = await _inner.GetTinyUrl(shortCode);

            if (success && tiny is { } _)
                await _cache.SetStringAsync(tiny.ShortCode, tiny.Uri.ToString(), _cacheConfig.Value);

            return (success, tiny);
        }
        catch {
            _logger.LogWarning("Failed to retrieve tiny url for {shortCode}", shortCode);
            return (false, null);
        }
    }
}