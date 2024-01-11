using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;

namespace SportSync.GraphQL;

public class Query
{
    [Authorize]
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<User> Me([Service(ServiceKind.Synchronized)] IDbContext dbContext, [Service] IUserIdentifierProvider userIdentifierProvider)
        => dbContext.Set<User>().Where(x => x.Id == userIdentifierProvider.UserId);
}
