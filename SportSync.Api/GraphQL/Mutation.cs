using SportSync.Application.Authentication;
using SportSync.Application.Users;

namespace SportSync.GraphQL;

public class Mutation
{
    public async Task<Domain.Core.Primitives.Result.Result<TokenResponse>> CreateUser([Service] CreateUserInputHandler inputHandler, CreateUserInput input, CancellationToken cancellationToken) =>
        await inputHandler.Handle(input, cancellationToken);
}