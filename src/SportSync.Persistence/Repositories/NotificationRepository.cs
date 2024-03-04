using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Persistence.Repositories;

public class NotificationRepository : QueryableGenericRepository<Notification, NotificationType>, INotificationRepository
{
    public NotificationRepository(IDbContext dbContext) : base(dbContext, NotificationType.PropertySelector)
    {
    }

    public Task<List<Notification>> GetByResourceIdAndType(Guid resourceId, NotificationTypeEnum type, CancellationToken cancellationToken)
    {
        return DbContext.Set<Notification>()
            .Where(x => x.ResourceId == resourceId && x.Type == type)
            .ToListAsync(cancellationToken);
    }
}