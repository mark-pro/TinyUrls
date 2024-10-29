using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;

namespace TinyUrls.Api.Minimal.Tests;

public class EndpointsTests(ServiceProviderFixture serviceProviderFixture
) : IClassFixture<ServiceProviderFixture>{

    private IServiceScope ServiceScope =>
        serviceProviderFixture.ServiceScope;
    
    private DefaultHttpContext CreateHttpContext() =>
        new() {
            RequestServices = ServiceScope.ServiceProvider
        };
    
    [Fact]
    public async Task Successful_Create() {
        // Arrange
        var createRequest = new CreateRequest(new Uri("https://example.com"));
        var config = ServiceScope.ServiceProvider
            .GetRequiredService<IOptions<ShortnerConfig>>();
        var shortner = ServiceScope.ServiceProvider
            .GetRequiredService<IShortner>();
        var dbContext = ServiceScope.ServiceProvider
            .GetRequiredService<TinyUrlDbContext>();

        var httpContext = CreateHttpContext();
        
        // Act
        var result = Endpoints.CreateShortenedUrl(createRequest, config, shortner, dbContext);
        await result.ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal((int) HttpStatusCode.Created, httpContext.Response.StatusCode);
        Assert.Matches($"^/{config.Value.GetRegex().ToString()[1..^1]}$", httpContext.Response.Headers["Location"]);
    }

    [Fact]
    public async Task Successful_Redirect() {
        // Arrange
        var dbContext = ServiceScope.ServiceProvider
            .GetRequiredService<TinyUrlDbContext>();
        var config = ServiceScope.ServiceProvider
            .GetRequiredService<IOptions<ShortnerConfig>>();
        var shortner = ServiceScope.ServiceProvider
            .GetRequiredService<IShortner>();
        var logger = ServiceScope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
        var tinyUrl = shortner.Shorten(config.Value, new Uri("https://example.com"));
        dbContext.TinyUrls.Add(tinyUrl);
        await dbContext.SaveChangesAsync();
        
        var httpContext = CreateHttpContext();
        
        // Act
        var result = Endpoints.RedirectFromShortCode(tinyUrl.ShortCode, dbContext, logger);
        await result.ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal((int) HttpStatusCode.PermanentRedirect, httpContext.Response.StatusCode);
        Assert.Equal(tinyUrl.Uri.ToString(), httpContext.Response.Headers["Location"]);
    }
}