using SportSync.Application.Authentication;
using SportSync.Application.Users;

namespace SportSync.GraphQL;

public class Mutation
{
    public async Task<TokenResponse> CreateUser([Service] CreateUserInputHandler inputHandler, CreateUserInput input) =>
        await inputHandler.Handle(input);
}