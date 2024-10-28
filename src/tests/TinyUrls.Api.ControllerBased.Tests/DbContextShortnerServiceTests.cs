using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TinyUrls.Api.Controllers.Services;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;

namespace TinyUrls.Api.ControllerBased;

public sealed class DbContextShortnerServiceTests(ServiceProviderFixture spFixture) :
    IClassFixture<ServiceProviderFixture> {

    private IServiceScope ServiceScope => spFixture.ServiceScope;
    
    [Fact]
    public async Task Shortner_DbContext_Create_Success() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("dbcontext_shortner");
        var config = ServiceScope.ServiceProvider
            .GetRequiredService<IOptions<ShortnerConfig>>();

        var expectedUri = new Uri("https://example.com");
        
        // Act
        var (success, tiny) = await service.CreateTinyUrl(expectedUri);
        
        // Assert
        Assert.True(success);
        Assert.Matches(config.Value.GetRegex(), tiny?.ShortCode);
        Assert.Equal(expectedUri, tiny?.Uri);
    }
    
    [Fact]
    public async Task Shortner_DbContext_Get_Success() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("dbcontext_shortner");
        var (_, expected) = await service.CreateTinyUrl(new ("https://example.com"));
        
        // Act
        var (success, tiny) = await service.GetTinyUrl(expected!.ShortCode);
        
        // Assert
        Assert.True(success);
        Assert.Equal(expected, tiny);
    }
}