﻿using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Events.GetDatesByDayOfWeek;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Types.Queries;

[ExtendObjectType("Query")]
public class TerminQuery
{
    public async Task<GetDatesByDayOfWeekResponse> GetDatesByDayOfWeek(
        [Service] GetDatesByDayOfWeekInputHandler inputHandler,
        [UseFluentValidation] GetDatesByDayOfWeekInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);

    [Authorize]
    [UseProjection]
    public IQueryable<TerminType> GetTermins(
        [Service] ITerminRepository repository,
        [Service] IUserIdentifierProvider userIdentifierProvider,
        DateTime date)
        => repository.GetQueryable(x => x.Players.Any(c => c.UserId == userIdentifierProvider.UserId && date == x.Date));

    [Authorize]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<TerminType> GetTerminById(
        [Service] ITerminRepository repository,
        [Service] IUserIdentifierProvider userIdentifierProvider,
        Guid id)
        => repository.GetQueryable(x => x.Id == id && x.Players.Any(c => c.UserId == userIdentifierProvider.UserId));
}
