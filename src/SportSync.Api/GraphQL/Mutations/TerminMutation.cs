using HotChocolate.Authorization;
using SportSync.Application.Termins.AnnounceTermin;
using SportSync.Application.Termins.SendTerminApplication;
using SportSync.Application.Termins.SetTerminAttendence;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class TerminMutation
{
    [Authorize]
    public async Task<SetTerminAttendenceResponse> SetTerminAttendence(
        [Service] SetTerminAttendenceRequestHandler requestHandler,
        SetTerminAttendenceInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<TerminType> AnnounceTermin(
        [Service] AnnounceTerminRequestHandler requestHandler,
        AnnounceTerminInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);


    [Authorize]
    public async Task<Result> SendTerminApplication(
        [Service] SendTerminApplicationRequestHandler requestHandler,
        SendTerminApplicationInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}