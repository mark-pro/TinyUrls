using Microsoft.EntityFrameworkCore;

namespace TinyUrls.Persistence;

public interface IDbContext<T> where T : DbContext, IDbContext<T> {
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}