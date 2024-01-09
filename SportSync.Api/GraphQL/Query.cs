using HotChocolate.Authorization;

namespace SportSync.GraphQL;

public class Query
{
    [Authorize]
    public string Test() => "Test";
}
