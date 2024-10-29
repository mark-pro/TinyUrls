using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using TinyUrls.Api.Controllers.Contracts;
using TinyUrls.Api.Controllers.Controllers;
using TinyUrls.Api.Controllers.Services;
using TinyUrls.Types;

namespace TinyUrls.Api.ControllerBased;

public sealed class TinyUrlsControllerTests(ServiceProviderFixture spFixture) 
    : IClassFixture<ServiceProviderFixture> {
    
    private IServiceScope ServiceScope => spFixture.ServiceScope;

    private ActionContext CreateActionContext() =>
        new () {
            HttpContext = new DefaultHttpContext {
                RequestServices = ServiceScope.ServiceProvider
            },
            ActionDescriptor = new ActionDescriptor(),
            ModelState = { },
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
        };

    [Fact]
    public async Task Redirect_Success() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("cached_shortner");
        var controller = new TinyUrlsController(service);
        var actionContext = CreateActionContext();
        
        // Act
        var result = await controller.RedirectFromShortCode(ShortCode.FromString("abc123"));
        await result.ExecuteResultAsync(actionContext);
        
        // Assert
        Assert.IsType<RedirectResult>(result);
        Assert.Equal((int) HttpStatusCode.PermanentRedirect, actionContext.HttpContext.Response.StatusCode);
    }
    
    [Fact]
    public async Task Redirect_Failure() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("poisoned_cached_shortner");
        var controller = new TinyUrlsController(service);
        var actionContext = CreateActionContext();
        
        // Act
        var result = await controller.RedirectFromShortCode(ShortCode.FromString("abc123"));
        await result.ExecuteResultAsync(actionContext);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
        Assert.Equal((int) HttpStatusCode.NotFound, actionContext.HttpContext.Response.StatusCode);
    }
    
    [Fact]
    public async Task Create_Success() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("cached_shortner");
        var controller = new TinyUrlsController(service);
        var actionContext = CreateActionContext();
        
        // Act
        var result = await controller.CreateShortenedUrl(new CreateRequest(new("https://example.com")));
        await result.ExecuteResultAsync(actionContext);
        
        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.Equal((int) HttpStatusCode.Created, actionContext.HttpContext.Response.StatusCode);
    }
    
    [Fact]
    public async Task Create_Failure() {
        // Arrange
        var service = ServiceScope.ServiceProvider
            .GetRequiredKeyedService<IShortnerService>("poisoned_cached_shortner");
        var controller = new TinyUrlsController(service);
        var actionContext = CreateActionContext();
        
        // Act
        var result = await controller.CreateShortenedUrl(new CreateRequest(new("https://example.com")));
        await result.ExecuteResultAsync(actionContext);
        
        // Assert
        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal((int) HttpStatusCode.InternalServerError, actionContext.HttpContext.Response.StatusCode);
    }
}