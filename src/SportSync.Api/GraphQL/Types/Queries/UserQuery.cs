using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Types.Queries;

[ExtendObjectType("Query")]
public class UserQuery
{
    [Authorize]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<UserType> Me([Service(ServiceKind.Synchronized)] IUserRepository repository, [Service] IUserIdentifierProvider userIdentifierProvider)
        => repository.GetQueryable(x => x.Id == userIdentifierProvider.UserId);
}