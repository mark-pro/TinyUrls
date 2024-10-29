namespace TinyUrls.Types;

public sealed record TinyUrlType {
    internal TinyUrlType(ShortCodeType shortCode, Uri uri) =>
        (ShortCode , Uri) = (shortCode, uri);
    public ShortCodeType ShortCode { get; set; } 
    public Uri Uri { get; set; }
}

public static class TinyUrl {
    
    public static TinyUrlType Create(ShortCodeType shortCode, Uri uri) =>
        new(shortCode, uri);
}