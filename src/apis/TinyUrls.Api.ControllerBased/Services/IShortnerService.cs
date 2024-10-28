using TinyUrls.Types;

namespace TinyUrls.Api.Controllers.Services;

public interface IShortnerService {
    ValueTask<(bool success, TinyUrlType? tinyUrl)> CreateTinyUrl(Uri uri);
    ValueTask<(bool success, TinyUrlType? tinyUrl)> GetTinyUrl(ShortCodeType shortCode);
}