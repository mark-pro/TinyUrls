namespace TinyUrl.Prelude.Tests;

using TinyUrls.Prelude.Shortner;

public sealed class ShortnerTests {
    record struct Config(ushort MaxLength, HashSet<char> Alphabet) : IShortnerConfig;
    
    [Fact]
    public void Shorten() {
        var shortner = new Shortner();
        var config = new Config(5, new("abc"));
        var uri = new Uri("https://example.com");
        var tinyUrl = shortner.Shorten(config, uri);
        
        Assert.All(tinyUrl.ShortCode.ToString(), (c, _) => Assert.Contains(c, config.Alphabet));
        Assert.Equal(config.MaxLength, tinyUrl.ShortCode.ToString().Length);
        Assert.Equal(uri, tinyUrl.Uri);
    }
}