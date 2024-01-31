using HotChocolate.Authorization;
using SportSync.Application.Termins.AnnounceTermin;
using SportSync.Application.Termins.SetTerminAttendence;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class TerminMutation
{
    [Authorize]
    public async Task<SetTerminAttendenceResponse> SetTerminAttendence(
        [Service] SetTerminAttendenceInputHandler inputHandler,
        SetTerminAttendenceInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<TerminType> AnnounceTermin(
        [Service] AnnounceTerminInputHandler inputHandler,
        AnnounceTerminInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);
}