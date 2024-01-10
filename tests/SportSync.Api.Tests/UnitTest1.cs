using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using SportSync.GraphQL;

namespace SportSync.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task SchemaChangeTest()
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
}