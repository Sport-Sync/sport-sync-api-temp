using AppAny.HotChocolate.FluentValidation;
using SportSync.Application.Authentication;
using SportSync.Application.Authentication.Login;
using SportSync.Application.Users.CreateUser;

namespace sport_sync.GraphQL.Types;

public class Mutation
{
    public async Task<TokenResponse> CreateUser(
        [Service] CreateUserInputHandler inputHandler,
        [UseFluentValidation] CreateUserInput input,
        CancellationToken cancellationToken) =>
            await inputHandler.Handle(input, cancellationToken);

    public async Task<TokenResponse> Login(
        [Service] LoginInputHandler inputHandler,
        [UseFluentValidation] LoginInput input,
        CancellationToken cancellationToken) =>
            await inputHandler.Handle(input, cancellationToken);
}