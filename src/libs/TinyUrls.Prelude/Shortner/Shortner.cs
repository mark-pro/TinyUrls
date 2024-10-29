namespace TinyUrls.Prelude.Shortner;

using Types;

public sealed class Shortner : IShortner  {
    public TinyUrlType Shorten(IShortnerConfig config, Uri uri) {
        var shortCode = string.Empty;
        for (ushort i = 0; i < config.MaxLength; i++)
            shortCode += config.Alphabet.ElementAt(Random.Shared.Next(config.Alphabet.Count));
        return TinyUrl.Create(ShortCode.FromString(shortCode), uri);
    }
}