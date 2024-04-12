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

    public Task<List<Notification>> GetForEntitySource(Guid entitySourceId, NotificationTypeEnum type, CancellationToken cancellationToken)
    {
        return DbContext.Set<Notification>()
            .Where(x => x.EntitySourceId == entitySourceId && x.Type == type)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Notification>> GetForEntitySource(Guid entitySourceId, NotificationTypeEnum type, Guid userId, CancellationToken cancellationToken)
    {
        return DbContext.Set<Notification>()
            .Where(x => x.EntitySourceId == entitySourceId && x.Type == type && x.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}