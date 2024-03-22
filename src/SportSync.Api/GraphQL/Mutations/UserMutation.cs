using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Authentication;
using SportSync.Application.Authentication.Login;
using SportSync.Application.Users.CreateUser;
using SportSync.Application.Users.UploadProfileImage;
using SportSync.Domain.Core.Primitives.Result;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class UserMutation
{
    public async Task<TokenResponse> CreateUser(
        [Service] CreateUserRequestHandler requestHandler,
        [UseFluentValidation] CreateUserInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    public async Task<TokenResponse> Login(
        [Service] LoginRequestHandler requestHandler,
        [UseFluentValidation] LoginInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> UploadUserProfileImage(
        [Service] UploadProfileImageRequestHandler requestHandler,
        UploadProfileImageInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}