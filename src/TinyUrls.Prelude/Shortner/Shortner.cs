namespace TinyUrls.Prelude.Shortner;

using Types;

public sealed class Shortner : IShortner  {
    public TinyUrlType Shorten(IShortnerConfig config, Uri uri) {
        var shortCode = CreateShortCode(config, string.Empty);
        return TinyUrl.Create(shortCode, uri);
        
        static ShortCodeType CreateShortCode(IShortnerConfig config, string acc) =>
            acc switch {
                var x when x.Length < config.MaxLength =>
                    CreateShortCode(config, acc + config.Alphabet.ElementAt(Random.Shared.Next(config.Alphabet.Count))),
                _ => ShortCode.FromString(acc)
            };
    }
}