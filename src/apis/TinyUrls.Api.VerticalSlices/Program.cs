using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyUrls.Api.VerticalSlices;
using TinyUrls.Persistence;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

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

builder.Services.AddSingleton<IShortner, Shortner>();
builder.Services.AddDbContext<IDbContext<TinyUrlDbContext>, TinyUrlDbContext>(options =>
    options.UseInMemoryDatabase("tiny_db")
        .EnableSensitiveDataLogging());

builder.Services.AddSingleton<IRequestMapper, RequestMapper>();

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.MapGet("/{shortCode}", async (ShortCodeType shortCode, IRequestMapper mapper, IMediator mediator) =>
    await mediator.Send(mapper.Map(shortCode)) switch {
        { } x => Results.Redirect(x.Uri.ToString(), true),
        _ => Results.NotFound()
    }
);

app.MapPut("/", async ([FromBody] CreateTinyUrlRequest request, IMediator mediator) =>
    await mediator.Send(request) switch {
        { } x => Results.Created($"{x.ShortCode}", x),
        _ => Results.StatusCode(500)
    }
);

app.Run();