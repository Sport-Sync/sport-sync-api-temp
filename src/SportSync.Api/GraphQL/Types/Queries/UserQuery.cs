using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace sport_sync.GraphQL.Types.Queries;

[ExtendObjectType("Query")]
public class UserQuery
{
    [Authorize]
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<User> Me([Service(ServiceKind.Synchronized)] IUserRepository repository, [Service] IUserIdentifierProvider userIdentifierProvider)
        => repository.Get().Where(x => x.Id == userIdentifierProvider.UserId);
}