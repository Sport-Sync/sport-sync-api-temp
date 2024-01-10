using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using SportSync.GraphQL;

namespace SportSync.Api.Tests;

public class SchemaChangeTest
{
    [Fact]
    public async Task Schema_Should_MatchSnapshot()
    {
        var schema = await new ServiceCollection()
            .AddGraphQLServer()
            .AddProjections()
            .AddFiltering()
            .AddAuthorization()
            .AddQueryType<Query>()
            .BuildSchemaAsync();

        schema.ToString().MatchSnapshot();
    }
}