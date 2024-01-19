using System.Linq.Expressions;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Persistence.Repositories;

public abstract class QueryableGenericRepository<TEntity, TQueryableType> : GenericRepository<TEntity>
    where TEntity : Entity
{
    private readonly Expression<Func<TEntity, TQueryableType>> _propertySelector;

    protected QueryableGenericRepository(IDbContext dbContext, Expression<Func<TEntity, TQueryableType>> propertySelector)
        : base(dbContext)
    {
        _propertySelector = propertySelector;
    }

    public virtual IQueryable<TQueryableType> GetQueryable(Expression<Func<TEntity, bool>> where = null)
    {
        var query = DbContext.Set<TEntity>().AsQueryable();

        if (where is not null)
        {
            query = query.Where(where);

        }
        return query.Select(_propertySelector);
    }
}