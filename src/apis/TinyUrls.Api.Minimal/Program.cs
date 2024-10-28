using Microsoft.EntityFrameworkCore;
using TinyUrls.Api.Minimal;
using TinyUrls.Prelude.Shortner;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IShortner, Shortner>();
builder.Services.AddDbContext<TinyUrlDbContext>(options =>
    options.UseInMemoryDatabase("tiny_db"));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapEndpoints();

if (!app.Environment.IsProduction())
    app.UseSwagger().UseSwaggerUI();

app.Run();