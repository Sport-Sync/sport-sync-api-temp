using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;

namespace SportSync.GraphQL;

public class Query
{
    [Authorize]
    public string Test() => "Test";

    [UseProjection]
    [UseFiltering]
    public IQueryable<User> GetUsers([Service] IDbContext dbContext) => dbContext.Set<User>();
}
