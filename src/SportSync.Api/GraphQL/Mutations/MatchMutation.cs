﻿using HotChocolate.Authorization;
using SportSync.Application.Matches.AcceptMatchApplication;
using SportSync.Application.Matches.AnnounceMatch;
using SportSync.Application.Matches.RejectMatchApplication;
using SportSync.Application.Matches.SendMatchApplication;
using SportSync.Application.Matches.SetMatchAttendance;
using SportSync.Domain.Core.Primitives.Result;
using MatchType = SportSync.Domain.Types.MatchType;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class MatchMutation
{
    [Authorize]
    public async Task<SetMatchAttendanceResponse> SetMatchAttendance(
        [Service] SetMatchAttendanceRequestHandler requestHandler,
        SetMatchAttendanceInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<MatchType> AnnounceMatch(
        [Service] AnnounceMatchRequestHandler requestHandler,
        AnnounceMatchInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> SendMatchApplication(
        [Service] SendMatchApplicationRequestHandler requestHandler,
        SendMatchApplicationInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> AcceptMatchApplication(
        [Service] AcceptMatchApplicationRequestHandler requestHandler,
        AcceptMatchApplicationInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> RejectMatchApplication(
        [Service] RejectMatchApplicationRequestHandler requestHandler,
        RejectMatchApplicationInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}