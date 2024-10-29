namespace TinyUrls.Api.Minimal.Tests;

using Microsoft.EntityFrameworkCore;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;
using Microsoft.Extensions.DependencyInjection;

public class ServiceProviderFixture : IDisposable {
    public IServiceScope ServiceScope { get; }
    
    public ServiceProviderFixture() {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<ShortnerConfig>(config => {
            config.MaxLength = 7;
            config.Alphabet = new("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        });
        services.AddSingleton<IShortner, Shortner>();
        services.AddDbContext<TinyUrlDbContext>(options => options.UseInMemoryDatabase("tiny_db"));
        ServiceScope = services.BuildServiceProvider().CreateScope();
    }

    public void Dispose() => ServiceScope?.Dispose();
}