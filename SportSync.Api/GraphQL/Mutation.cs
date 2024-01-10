using System.ComponentModel.DataAnnotations;
using SportSync.Application.Authentication;
using SportSync.Application.Authentication.Login;
using SportSync.Application.Users.CreateUser;

namespace SportSync.GraphQL;

public class Mutation
{
    public async Task<TokenResponse> CreateUser([Service] CreateUserMutationHandler mutationHandler, CreateUserInput input, CancellationToken cancellationToken) =>
        await mutationHandler.Handle(input, cancellationToken);

    public async Task<TokenResponse> Login([Service] LoginMutationHandler mutationHandler, LoginInput input, CancellationToken cancellationToken) =>
        await mutationHandler.Handle(input, cancellationToken);
}