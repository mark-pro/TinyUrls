using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TinyUrls.Persistence;

public static class DbSetExtensions {
    public static Option<T> FirstOrOptional<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> pred) where T : class {
        try {
            return dbSet.FirstOrDefault(pred) is { } entity ? Option<T>.Some(entity) : Option<T>.None;
        }
        catch {
            return Option<T>.None;
        }
    }
}