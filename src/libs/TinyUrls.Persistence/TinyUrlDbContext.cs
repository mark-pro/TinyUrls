using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TinyUrls.Prelude.Shortner;
using TinyUrls.Types;

namespace TinyUrls.Persistence;

public record ShortnerConfig : IShortnerConfig {
    public required ushort MaxLength { get; set; }
    public required HashSet<char> Alphabet { get; set; }
};

public class TinyUrlDbContext(IOptions<ShortnerConfig> config, DbContextOptions options) 
    : DbContext(options), IDbContext<TinyUrlDbContext> {
    
    public virtual DbSet<TinyUrlType> TinyUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var tinyUrl = modelBuilder.Entity<TinyUrlType>();
        tinyUrl.HasKey(t => t.ShortCode);
        tinyUrl.Property(t => t.ShortCode)
            .HasConversion(t => t.ToString(), s => ShortCode.FromString(s))
            .HasMaxLength(config.Value.MaxLength);
        tinyUrl.Property(t => t.Uri)
            .HasConversion(uri => uri.ToString(), s => new Uri(s));
    }
}