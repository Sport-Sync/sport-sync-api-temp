using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task EnsureUserIsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var user = await DbContext.Set<Event>()
            .Where(x => x.Id == eventId)
            .SelectMany(x => x.Members)
            .Where(m => m.IsAdmin)
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

        if (user is null)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }
    }
}
