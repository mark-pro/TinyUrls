using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TinyUrls.Api.Controllers.Services;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddOptions<ShortnerConfig>()
    .Configure(config => {
        var section = builder.Configuration.GetRequiredSection("ShortnerConfig");
        config.Alphabet = [..section["Alphabet"]!];
        config.MaxLength = section.GetValue<ushort>("MaxLength");
    })
    .Validate(config => config is { Alphabet.Count: > 0 }, "Alphabet must not be empty")
    .Validate(config => config is { MaxLength: > 0 }, "MaxLength must be greater than 0")
    .ValidateOnStart();

builder.Services.Configure<DistributedCacheEntryOptions>(config => {
    config.SlidingExpiration = TimeSpan.FromDays(1);
});

builder.Services.AddScoped<IShortner, Shortner>();

builder.Services.AddDbContext<TinyUrlDbContext>(options =>
    options.UseInMemoryDatabase("tiny_db"));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddKeyedScoped<IShortnerService, DbContextShortnerService>("db_shortner")
    .AddKeyedScoped<IShortnerService, CacheShortnerService>("cached_shortner", (sp, _) =>
        new CacheShortnerService(sp.GetRequiredService<IOptions<DistributedCacheEntryOptions>>(),
            sp.GetRequiredService<IDistributedCache>(),
            sp.GetRequiredKeyedService<IShortnerService>("db_shortner"),
            sp.GetRequiredService<ILogger<CacheShortnerService>>()));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
