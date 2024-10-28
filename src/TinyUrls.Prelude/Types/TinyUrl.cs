namespace TinyUrls.Types;

public sealed record TinyUrlType(ShortCodeType ShortCode, Uri Uri);

public static class TinyUrl {
    public static TinyUrlType Create(ShortCodeType shortCode, Uri uri) =>
        new(shortCode, uri);
}