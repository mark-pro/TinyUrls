namespace TinyUrls.Prelude.Shortner;

using Types;

public interface IShortner {
    TinyUrlType Shorten(IShortnerConfig config, Uri uri);
}