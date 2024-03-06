using System.Linq.Expressions;

namespace SportSync.Domain.Repositories;

public interface IQueryableRepository<TEntity, TQueryableType>
{
    IQueryable<TQueryableType> GetQueryable(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>>[] includes = null);
}