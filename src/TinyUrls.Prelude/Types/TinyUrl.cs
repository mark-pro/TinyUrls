namespace TinyUrls.Types;

public sealed class TinyUrlType {
    internal TinyUrlType(ShortCodeType shortCode, Uri uri) =>
        (ShortCode, Uri) = (shortCode, uri);
    
    public ShortCodeType ShortCode { get; init; }
    public Uri Uri { get; init; }
}

public static class TinyUrl {
    public static TinyUrlType Create(ShortCodeType shortCode, Uri uri) =>
        new(shortCode, uri);
}