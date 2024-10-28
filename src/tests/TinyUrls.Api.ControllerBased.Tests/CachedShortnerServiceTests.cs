using Microsoft.Extensions.DependencyInjection;
using TinyUrls.Api.Controllers.Services;
using TinyUrls.Types;

namespace TinyUrls.Api.ControllerBased;

public sealed class CachedShortnerServiceTests(ServiceProviderFixture spFixture)
    : IClassFixture<ServiceProviderFixture> {
    private IServiceScope ServiceScope => spFixture.ServiceScope;
    
    [Fact]
    public async Task Shortner_Cache_Create_Success() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("cached_shortner");
        var expected = TinyUrl.Create(ShortCode.FromString("abc123"), new Uri("https://example.com"));
        
        // Act
        var (success, tiny) = await service.CreateTinyUrl(new("https://example.com"));
        
        // Assert
        Assert.True(success);
        Assert.Equal(expected, tiny);
    }

    [Fact]
    public async Task Shortner_Cache_Get_Success() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("cached_shortner");
        var expected = TinyUrl.Create(ShortCode.FromString("abc123"), new Uri("https://example.com"));
        
        // Act
        var (success, tiny) = await service.GetTinyUrl(ShortCode.FromString("abc123"));
        
        // Assert
        Assert.True(success);
        Assert.Equal(expected, tiny);
    }
    
    [Fact]
    public async Task Shortner_Cached_Create_Failure() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("poisoned_cached_shortner");
        
        // Act
        var (success, tiny) = await service.CreateTinyUrl(new("https://example.com"));
        
        // Assert
        Assert.False(success);
        Assert.Null(tiny);
    }
    
    [Fact]
    public async Task Shortner_Cached_Get_Failure() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("poisoned_cached_shortner");
        
        // Act
        var (success, tiny) = await service.GetTinyUrl(ShortCode.FromString("abc123"));
        
        // Assert
        Assert.False(success);
        Assert.Null(tiny);
    }
}