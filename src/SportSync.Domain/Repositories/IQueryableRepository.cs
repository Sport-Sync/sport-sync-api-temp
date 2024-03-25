using System.Linq.Expressions;

namespace SportSync.Domain.Repositories;

public interface IQueryableRepository<TEntity, TQueryableType>
{
    IQueryable<TQueryableType> GetQueryable(Expression<Func<TEntity, bool>> where = null);

    IQueryable<TProjectionType> GetQueryable<TProjectionType>(Expression<Func<TEntity, TProjectionType>> propertySelector, Expression<Func<TEntity, bool>> where = null);
}