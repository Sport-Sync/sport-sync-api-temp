using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Events.GetDatesByDayOfWeek;
using SportSync.Domain.DtoTypes;
using SportSync.Domain.Repositories;

namespace sport_sync.GraphQL.Types.Queries;

[ExtendObjectType("Query")]
public class EventQuery
{
    public async Task<GetDatesByDayOfWeekResponse> GetDatesByDayOfWeek(
        [Service] GetDatesByDayOfWeekInputHandler inputHandler,
        [UseFluentValidation] GetDatesByDayOfWeekInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);

    [Authorize]
    [UseProjection]
    public IQueryable<TerminType> GetTermins(
        [Service] IEventRepository repository,
        [Service] IUserIdentifierProvider userIdentifierProvider,
        DateTime date)
        => repository.GetTermins(userIdentifierProvider.UserId, date);
}
