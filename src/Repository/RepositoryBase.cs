using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;

namespace Repository;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected RepositoryContext RepositoryContext { get; set; }
    
    protected RepositoryBase(RepositoryContext repositoryContext) => 
        RepositoryContext = repositoryContext;
    
    public IQueryable<T> FindAll(bool trackChanges) =>
        !trackChanges
            ? RepositoryContext.Set<T>()
                .AsNoTracking()
            : RepositoryContext.Set<T>();
    
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
        !trackChanges
            ? RepositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking()
            : RepositoryContext.Set<T>()
                .Where(expression);
    
    public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
    
    public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);

    public void Update(T entity)
    {
        if (entity is BaseEntity) (entity as BaseEntity)!.ModifiedDate = DateTime.Now;
        RepositoryContext.Set<T>().Update(entity);
    }
}