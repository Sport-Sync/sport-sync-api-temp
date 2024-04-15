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
            .Include(e => e.Invitations)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken));
    }

    public async Task<Maybe<Event>> GetByEventInvitationIdAsync(Guid eventInvitationId, CancellationToken cancellationToken)
    {
        return Maybe<Event>.From(await DbContext.Set<Event>()
            .Include(e => e.Members)
            .Include(t => t.Schedules)
            .Include(e => e.Invitations)
            .Where(x => x.Invitations.Any(i => i.Id == eventInvitationId))
            .FirstOrDefaultAsync(cancellationToken));
    }

    public async Task<List<EventInvitation>> GetPendingInvitations(Guid eventId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<EventInvitation>()
            .Where(i => i.EventId == eventId && i.CompletedOnUtc == null)
            .ToListAsync(cancellationToken);
    }

    public async Task EnsureUserIsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        if (await IsAdminOnEvent(eventId, userId, cancellationToken) != true)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }
    }

    public async Task<bool> IsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Event>()
            .Where(x => x.Id == eventId)
            .SelectMany(x => x.Members)
            .Where(m => m.IsAdmin)
            .Select(m => m.UserId)
            .CountAsync(id => id == userId, cancellationToken) > 0;
    }

    public async Task<List<Guid>> GetEventIdsThatUserIsAdminOn(Guid userId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Event>()
            .SelectMany(x => x.Members)
            .Where(m => m.IsAdmin && m.UserId == userId)
            .Select(m => m.EventId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Event>> GetEventsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Event>()
            .Where(e => e.Members.Select(m => m.UserId).Contains(userId))
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }
}
