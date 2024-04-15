using HotChocolate.Authorization;
using SportSync.Application.Team.CreateTeam;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class TeamMutation
{
    [Authorize]
    public async Task<TeamType> CreateTeam(
        [Service] CreateTeamRequestHandler requestHandler,
        CreateTeamInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}