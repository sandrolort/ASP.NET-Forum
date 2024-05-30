using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Repository.RepositoryExtensions;

public static class GenericExtensions
{
    public static IQueryable<T> TrackChanges<T>(this IQueryable<T> query, bool track) where T : class => 
        track ? query : query.AsNoTracking();

    public static IQueryable<TEntity> ConditionalInclude<TEntity, TProperty>(this IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath, bool include)
        where TEntity : class where TProperty : class =>
        include ? query.Include(navigationPropertyPath) : query;
}