﻿using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IEventRepository
{
    Task<Maybe<Event>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Insert(Event @event);
    Task EnsureUserIsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken);
}
