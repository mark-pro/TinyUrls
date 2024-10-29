open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Options
open TinyUrls.Api.FSharp.Shortner
open TinyUrls.Types

type TinyUrlsContext = TinyUrls.Persistence.TinyUrlDbContext
type CreateRequest = { Url: Uri }

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    
    builder.Services.AddSingleton<IOptions<ShortnerConfig>>(Options.Create(defaultConfig)) |> ignore
    builder.Services.AddDbContext<TinyUrlsContext>(
        fun options -> options.UseInMemoryDatabase("tiny_db") |> ignore
    ) |> ignore
    
    let app = builder.Build()
    
    app.MapGet("/{shortCode}", Func<ShortCodeType, TinyUrlsContext, IResult Task>(
        fun shortCode context ->
            task {
                let! result = context.TinyUrls.FirstOrDefaultAsync(fun tiny -> tiny.ShortCode = shortCode)
                
                return match result with
                       | null -> Results.NotFound()
                       | url -> Results.Redirect(url.Uri |> string, true, false)
            }
        )) |> ignore
    
    app.MapPut("/", Func<CreateRequest, TinyUrlsContext, ShortnerConfig IOptions, IResult Task>(
        fun request context config ->
            task {
                let shortCode = createShortCode config.Value request.Url
                context.TinyUrls.Add(shortCode) |> ignore
                let! _ = context.SaveChangesAsync()
                return Results.Created(shortCode.ShortCode, shortCode)
            }
        )) |> ignore

    app.Run()

    0

