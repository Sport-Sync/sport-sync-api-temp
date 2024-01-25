using HotChocolate.Authorization;
using SportSync.Application.Termins.SetTerminAttendence;

namespace sport_sync.GraphQL.Types.Mutations;

[ExtendObjectType("Mutation")]
public class TerminMutation
{
    [Authorize]
    public async Task<SetTerminAttendenceResponse> SetTerminAttendence(
        [Service] SetTerminAttendenceInputHandler inputHandler,
        SetTerminAttendenceInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);
}