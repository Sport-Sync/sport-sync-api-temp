using HotChocolate.Authorization;
using SportSync.Application.Core.Common;
using SportSync.Application.Users.GetByPhoneNumbers;
using SportSync.Application.Users.GetCurrentUser;
using SportSync.Application.Users.GetProfileImageUrl;
using SportSync.Application.Users.GetUserProfile;
using SportSync.Application.Users.GetUsers;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class UserQuery
{
    [Authorize]
    public async Task<UserType> Me(
        [Service] GetCurrentUserRequestHandler requestHandler,
        CancellationToken cancellationToken) => await requestHandler.Handle(cancellationToken);

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
    public async Task<UserProfileResponse> GetUserProfile(
        [Service] GetUserProfileRequestHandler requestHandler,
        GetUserProfileInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<PagedList<UserType>> GetUsers(
        [Service] GetUsersRequestHandler requestHandler,
        GetUsersInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}