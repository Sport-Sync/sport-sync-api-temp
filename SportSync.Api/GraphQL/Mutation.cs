using sport_sync.Contracts.Authentication;
using SportSync.Application.Users;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;

namespace sport_sync.GraphQL;

public class Mutation
{
    public async Task<TokenResponse> CreateUser([Service] CreateUserRequestHandler createUserRequest, RegisterRequest request) =>
        await Result.Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(request => new CreateUserCommand(request.FirstName, request.LastName, request.Email, request.Password))
            .Bind(command => Mediator.Send(command))
            .Match(Ok, BadRequest);
    //{
    //    var item = userRepository.CreateUser(createUserInput);

    //    return new CreateUserPayload(item);
    //}
}