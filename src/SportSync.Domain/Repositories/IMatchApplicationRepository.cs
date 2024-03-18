﻿using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IMatchApplicationRepository
{
    Task<Maybe<MatchApplication>> GetByIdAsync(Guid matchApplicationId, CancellationToken cancellationToken);
    Task<List<MatchApplication>> GetByMatchIdAsync(Guid matchId, CancellationToken cancellationToken);
    void Insert(MatchApplication matchApplication);
}