﻿using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Users.GetByPhoneNumbers;
using SportSync.Application.Users.GetProfileImageUrl;
using SportSync.Application.Users.GetUserProfile;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class UserQuery
{
    [Authorize]
    public Guid GetCurrentUserId([Service] IUserIdentifierProvider userIdentifierProvider) => userIdentifierProvider.UserId;

    [Authorize]
    public async Task<GetPhoneBookUsersResponse> GetPhoneBookUsers(
        [Service] GetPhoneBookUsersRequestHandler requestHandler,
        GetPhoneBookUsersInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<ProfileImageUrlResponse> GetUserProfileImageUrl(
        [Service] ProfileImageUrlRequestHandler requestHandler,
        ProfileImageUrlInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<UserProfileType> GetUserProfile(
        [Service] GetUserProfileRequestHandler requestHandler,
        GetUserProfileInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}