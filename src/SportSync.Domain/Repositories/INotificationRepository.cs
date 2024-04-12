using System.Linq.Expressions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Repositories;

public interface INotificationRepository
{
    Task<Maybe<Notification>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    IQueryable<Notification> Where(Expression<Func<Notification, bool>> predicate);
    Task<List<Notification>> GetForEntitySource(Guid entitySourceId, NotificationTypeEnum type, CancellationToken cancellationToken);
    Task<List<Notification>> GetForEntitySource(Guid entitySourceId, NotificationTypeEnum type, Guid userId, CancellationToken cancellationToken);
    void Insert(Notification notification);
    void InsertRange(IReadOnlyCollection<Notification> entities);
    void Remove(Notification notification);
}