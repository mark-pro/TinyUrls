using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TinyUrls.Api.Controllers.Services;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

namespace TinyUrls.Api.ControllerBased;

public sealed class ServiceProviderFixture : IDisposable {
    public IServiceScope ServiceScope { get; }

    private static TinyUrlType[] Data = [
        TinyUrl.Create(ShortCode.FromString("abc123"), new("https://example.com"))
    ];
    
    public ServiceProviderFixture() {
        var services = new ServiceCollection();
        services.Configure<DistributedCacheEntryOptions>(config => {
            config.AbsoluteExpirationRelativeToNow = TimeSpan.FromMicroseconds(1);
        });
        services.Configure<ShortnerConfig>(config => {
            config.MaxLength = 7;
            config.Alphabet = new("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        });
        services.AddKeyedScoped<IDbContext<TinyUrlDbContext>, TestDbContext>("dbcontext", (_, _) => {
            var context = new TestDbContext();
            context.TinyUrls.AddRange(Data);
            context.SaveChanges();
            return context;
        });
        
        services.AddKeyedScoped<IDbContext<TinyUrlDbContext>, PoisonedTestDbContext>("poisoned_dbcontext");
        
        services.AddControllers();
        
        services.AddLogging();
        services.AddSingleton<IShortner, Shortner>();
        
        services.AddDistributedMemoryCache();
        
        services.AddKeyedSingleton<IShortnerService>("mocked_shortner", (_, _) => {
            var mock = new Mock<IShortnerService>();
            mock.Setup(s => s.CreateTinyUrl(It.IsAny<Uri>()))
                .Returns(() =>
                    ValueTask.FromResult<(bool, TinyUrlType?)>((true,
                        TinyUrl.Create(ShortCode.FromString("abc123"), new("https://example.com")))));
            mock.Setup(s => s.GetTinyUrl(It.IsAny<ShortCodeType>()))
                .Returns(() =>
                    ValueTask.FromResult<(bool, TinyUrlType?)>(
                        (true, TinyUrl.Create(ShortCode.FromString("abc123"), new("https://example.com")))));

            return mock.Object;
        });
        
        services.AddKeyedSingleton<IShortnerService>("poisoned_mocked_shortner", (_, _) => {
            var mock = new Mock<IShortnerService>();
            mock.Setup(s => s.CreateTinyUrl(It.IsAny<Uri>()))
                .Returns(() => ValueTask.FromResult<(bool, TinyUrlType?)>((false, null)));
            mock.Setup(s => s.GetTinyUrl(It.IsAny<ShortCodeType>()))
                .Returns(() => ValueTask.FromResult<(bool, TinyUrlType?)>((false, null)));
            return mock.Object;
        });
        
        services.AddKeyedScoped<IShortnerService>("cached_shortner", (sp, _) =>
            new CacheShortnerService(sp.GetRequiredService<IOptions<DistributedCacheEntryOptions>>(),
                sp.GetRequiredService<IDistributedCache>(),
                sp.GetRequiredKeyedService<IShortnerService>("mocked_shortner"),
                sp.GetRequiredService<ILogger<CacheShortnerService>>()));
        
        services.AddKeyedScoped<IShortnerService>("poisoned_cached_shortner", (sp, _) =>
            new CacheShortnerService(sp.GetRequiredService<IOptions<DistributedCacheEntryOptions>>(),
                sp.GetRequiredService<IDistributedCache>(),
                sp.GetRequiredKeyedService<IShortnerService>("poisoned_mocked_shortner"),
                sp.GetRequiredService<ILogger<CacheShortnerService>>()));
        
        services.AddKeyedScoped<IShortnerService>("dbcontext_shortner", (sp, _) =>
            new DbContextShortnerService(
                sp.GetRequiredService<IShortner>(),
                sp.GetRequiredService<IOptions<ShortnerConfig>>(),
                sp.GetRequiredKeyedService<IDbContext<TinyUrlDbContext>>("dbcontext"),
                sp.GetRequiredService<ILogger<DbContextShortnerService>>()));
        
        services.AddKeyedScoped<IShortnerService>("poisoned_dbcontext_shortner", (sp, _) =>
            new DbContextShortnerService(
                sp.GetRequiredService<IShortner>(),
                sp.GetRequiredService<IOptions<ShortnerConfig>>(),
                sp.GetRequiredKeyedService<IDbContext<TinyUrlDbContext>>("poisoned_dbcontext"),
                sp.GetRequiredService<ILogger<DbContextShortnerService>>()));
        
        ServiceScope = services.BuildServiceProvider().CreateScope();
    }
    
    public void Dispose() {
        ServiceScope.Dispose();
    }
    
    private sealed class TestDbContext() 
        : TinyUrlDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("tiny_db").Options), IDbContext<TestDbContext> { }

    private sealed class PoisonedTestDbContext()
        : TinyUrlDbContext(new DbContextOptionsBuilder<PoisonedTestDbContext>().UseInMemoryDatabase("poisoned_tiny_db").Options),
            IDbContext<PoisonedTestDbContext> {

        public override int SaveChanges() {
            throw new DbUpdateException();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            throw new DbUpdateException();
        }
    }
}