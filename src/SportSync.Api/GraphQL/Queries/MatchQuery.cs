using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Events.GetDatesByDayOfWeek;
using SportSync.Application.Matches.GetAnnouncedMatches;
using SportSync.Application.Matches.GetMatchById;
using SportSync.Application.Matches.GetMatches;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class MatchQuery
{
    public async Task<GetDatesByDayOfWeekResponse> GetDatesByDayOfWeek(
        [Service] GetDatesByDayOfWeekRequestHandler requestHandler,
        [UseFluentValidation] GetDatesByDayOfWeekInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<GetMatchesResponse> GetMatches(
        [Service] GetMatchesRequestHandler requestHandler,
        GetMatchesInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<GetMatchByIdResponse> GetMatchById(
        [Service] GetMatchByIdRequestHandler requestHandler,
        GetMatchByIdInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<GetAnnouncedMatchesResponse> GetAnnouncedMatches(
        [Service] GetAnnouncedMatchesRequestHandler requestHandler,
        GetAnnouncedMatchesInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}
