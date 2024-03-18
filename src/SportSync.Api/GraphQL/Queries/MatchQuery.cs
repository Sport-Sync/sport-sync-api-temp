using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Events.GetDatesByDayOfWeek;
using SportSync.Application.Matches.GetAnnouncedMatches;
using SportSync.Application.Matches.GetMatchById;
using SportSync.Domain.Repositories;
using MatchType = SportSync.Domain.Types.MatchType;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class MatchQuery
{
    public async Task<GetDatesByDayOfWeekResponse> GetDatesByDayOfWeek(
        [Service] GetDatesByDayOfWeekRequestHandler requestHandler,
        [UseFluentValidation] GetDatesByDayOfWeekInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    [UseProjection]
    public IQueryable<MatchType> GetMatches(
        [Service] IMatchRepository repository,
        [Service] IUserIdentifierProvider userIdentifierProvider,
        DateTime date)
        => repository.GetQueryable(x => x.Players.Any(c => c.UserId == userIdentifierProvider.UserId && date.Date == x.Date.Date));

    [Authorize]
    [UseProjection]
    public async Task<GetMatchByIdResponse> GetMatchById(
        [Service] GetMatchByIdRequestHandler requestHandler,
        GetMatchByIdInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<GetAnnouncedMatchResponse> GetAnnouncedMatches(
        [Service] GetAnnouncedMatchesRequestHandler requestHandler,
        GetAnnouncedMatchesInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}
