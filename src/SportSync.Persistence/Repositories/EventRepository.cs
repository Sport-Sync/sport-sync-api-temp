using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task<Maybe<Event>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Maybe<Event>.From(await DbContext.Set<Event>()
            .Include(e => e.Members)
            .Include(t => t.Schedules)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken));
    }

    public async Task<List<EventInvitation>> GetPendingInvitations(Guid eventId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<EventInvitation>()
            .Where(i => i.EventId == eventId && i.CompletedOnUtc == null)
            .ToListAsync(cancellationToken);
    }

    public async Task EnsureUserIsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var user = await DbContext.Set<Event>()
            .Where(x => x.Id == eventId)
            .SelectMany(x => x.Members)
            .Where(m => m.IsAdmin)
            .Select(m => m.UserId)
            .FirstOrDefaultAsync(id => id == userId, cancellationToken);

        if (user == Guid.Empty)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }
    }
}
