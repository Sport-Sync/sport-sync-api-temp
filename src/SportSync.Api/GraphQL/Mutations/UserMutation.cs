using AppAny.HotChocolate.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SportSync.Application.Authentication;
using SportSync.Application.Authentication.Login;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Users.CreateUser;
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

    public async Task<Result> UploadUserProfileImageFile([FromServices] IBlobStorageService blobStorageService, IFile file)
    {
        var fileName = file.Name;
        var fileSize = file.Length;

        await blobStorageService.UploadFile(Guid.NewGuid().ToString(), file);

        return Result.Success();
    }

    public class UploadInput
    {
        [GraphQLType(typeof(NonNullType<UploadType>))]
        public IFile File { get; set; }
    }
}