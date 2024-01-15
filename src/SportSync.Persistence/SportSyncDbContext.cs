using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Events;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Persistence.Configurations;

namespace SportSync.Persistence;

public class SportSyncDbContext : DbContext, IDbContext, IUnitOfWork
{
    private readonly IDateTime _dateTime;
    private readonly IMediator _mediator;

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventMember> EventMembers { get; set; }

    public SportSyncDbContext(DbContextOptions options, IDateTime dateTime, IMediator mediator)
        : base(options)
    {
        _dateTime = dateTime;
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new EventMemberConfiguration());
    }

    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity
        => base.Set<TEntity>();

    public async Task<Maybe<TEntity>> GetByIdAsync<TEntity>(Guid id)
        where TEntity : Entity
        => id == Guid.Empty ?
            Maybe<TEntity>.None :
            Maybe<TEntity>.From(await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id));

    public void Insert<TEntity>(TEntity entity)
        where TEntity : Entity
        => Set<TEntity>().Add(entity);

    public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity
        => Set<TEntity>().AddRange(entities);

    public new void Remove<TEntity>(TEntity entity)
        where TEntity : Entity
        => Set<TEntity>().Remove(entity);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime utcNow = _dateTime.UtcNow;

        UpdateAuditableEntities(utcNow);

        UpdateSoftDeletableEntities(utcNow);

        await PublishDomainEvents(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<IAuditableEntity> entityEntry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(nameof(IAuditableEntity.CreatedOnUtc)).CurrentValue = utcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(nameof(IAuditableEntity.ModifiedOnUtc)).CurrentValue = utcNow;
            }
        }
    }

    private void UpdateSoftDeletableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<ISoftDeletableEntity> entityEntry in ChangeTracker.Entries<ISoftDeletableEntity>())
        {
            if (entityEntry.State != EntityState.Deleted)
            {
                continue;
            }

            entityEntry.Property(nameof(ISoftDeletableEntity.DeletedOnUtc)).CurrentValue = utcNow;

            entityEntry.Property(nameof(ISoftDeletableEntity.Deleted)).CurrentValue = true;

            entityEntry.State = EntityState.Modified;

            UpdateDeletedEntityEntryReferencesToUnchanged(entityEntry);
        }
    }

    private static void UpdateDeletedEntityEntryReferencesToUnchanged(EntityEntry entityEntry)
    {
        if (!entityEntry.References.Any())
        {
            return;
        }

        foreach (ReferenceEntry referenceEntry in entityEntry.References.Where(r => r.TargetEntry.State == EntityState.Deleted))
        {
            referenceEntry.TargetEntry.State = EntityState.Unchanged;

            UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
        }
    }
    
    private async Task PublishDomainEvents(CancellationToken cancellationToken)
    {
        List<EntityEntry<AggregateRoot>> aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entityEntry => entityEntry.Entity.DomainEvents.Any())
            .ToList();

        List<IDomainEvent> domainEvents = aggregateRoots.SelectMany(entityEntry => entityEntry.Entity.DomainEvents).ToList();

        aggregateRoots.ForEach(entityEntry => entityEntry.Entity.ClearDomainEvents());

        IEnumerable<Task> tasks = domainEvents.Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);
    }
}