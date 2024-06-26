﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Maybe;

namespace SportSync.Persistence.Repositories;

/// <summary>
/// Represents the generic repository with the most common repository methods.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public abstract class GenericRepository<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    protected GenericRepository(IDbContext dbContext) => DbContext = dbContext;

    /// <summary>
    /// Gets the database context.
    /// </summary>
    protected IDbContext DbContext { get; }

    /// <summary>
    /// Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The maybe instance that may contain the entity with the specified identifier.</returns>
    public virtual async Task<Maybe<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken) => await DbContext.GetByIdAsync<TEntity>(id, cancellationToken);

    /// <summary>
    /// Inserts the specified entity into the database.
    /// </summary>
    /// <param name="entity">The entity to be inserted into the database.</param>
    public void Insert(TEntity entity) => DbContext.Insert(entity);

    /// <summary>
    /// Inserts the specified entities to the database.
    /// </summary>
    /// <param name="entities">The entities to be inserted into the database.</param>
    public void InsertRange(IReadOnlyCollection<TEntity> entities) => DbContext.InsertRange(entities);

    /// <summary>
    /// Updates the specified entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    public void Update(TEntity entity) => DbContext.Set<TEntity>().Update(entity);

    /// <summary>
    /// Removes the specified entity from the database.
    /// </summary>
    /// <param name="entity">The entity to be removed from the database.</param>
    public void Remove(TEntity entity) => DbContext.Remove(entity);

    /// <summary>
    /// Checks if any entity meets the specified condition
    /// </summary>
    /// <param name="predicate">The condition</param>
    /// <returns>True if any entity meets the specified condition, otherwise false.</returns>
    protected async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) =>
        await DbContext.Set<TEntity>().AnyAsync(predicate);

    /// <summary>
    /// Gets the first entity that meets the specified condition.
    /// </summary>
    /// <param name="predicate">The condition.</param>
    /// <returns>The maybe instance that may contain the first entity that meets the specified condition.</returns>
    protected async Task<Maybe<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) =>
        await DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);

    /// <summary>
    /// Gets the entities that meets the specified condition.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate) =>
        DbContext.Set<TEntity>().Where(predicate);
}
