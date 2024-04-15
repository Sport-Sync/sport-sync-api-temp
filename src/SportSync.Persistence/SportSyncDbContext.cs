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
using SportSync.Persistence.Configurations;

namespace SportSync.Persistence;

public class SportSyncDbContext : DbContext, IDbContext, IUnitOfWork
{
    private readonly IDateTime _dateTime;
    private readonly IPublisher _publisher;

    public SportSyncDbContext(DbContextOptions options, IDateTime dateTime, IPublisher publisher)
        : base(options)
    {
        _dateTime = dateTime;
        _publisher = publisher;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name });
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new EventMemberConfiguration());
        modelBuilder.ApplyConfiguration(new MatchConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new EventScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new FriendshipRequestConfiguration());
        modelBuilder.ApplyConfiguration(new FriendshipConfiguration());
        modelBuilder.ApplyConfiguration(new MatchApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new MatchAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new EventInvitationConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());

        //var entityTypes = Assembly.GetExecutingAssembly()
        //    .GetTypes()
        //    .Where(t => typeof(Entity).IsAssignableFrom(t) && !t.IsInterface);

        //foreach (var entityType in entityTypes)
        //{
        //    // Get the method MakeGenericMethod for ValueGeneratedNever
        //    var method = typeof(EntityTypeBuilder).GetMethod(nameof(EntityTypeBuilder.Property))
        //        .MakeGenericMethod(entityType.GetProperty(nameof(Entity.Id)).PropertyType);

        //    // Invoke the method with arguments to configure the property
        //    method.Invoke(modelBuilder.Entity(entityType), new object[] { entityType.GetProperty(nameof(Entity.Id)) });
        //}
    }

    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity
        => base.Set<TEntity>();

    public async Task<Maybe<TEntity>> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken)
        where TEntity : Entity
        => id == Guid.Empty ?
            Maybe<TEntity>.None :
            Maybe<TEntity>.From(await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken));

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

        await PublishDomainEvents(cancellationToken);

        UpdateAuditableEntities(utcNow);
        UpdateSoftDeletableEntities(utcNow);

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
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

        foreach (ReferenceEntry referenceEntry in entityEntry.References.Where(r => 
                     r.TargetEntry != null && r.TargetEntry?.State == EntityState.Deleted))
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

        IEnumerable<Task> tasks = domainEvents.Select(domainEvent => _publisher.Publish(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);
    }
}