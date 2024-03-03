using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(IDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Notification>> GetByResourceIdAndType(Guid resourceId, NotificationType type, CancellationToken cancellationToken)
    {
        return DbContext.Set<Notification>()
            .Where(x => x.ResourceId == resourceId && x.Type == type)
            .ToListAsync(cancellationToken);
    }
}