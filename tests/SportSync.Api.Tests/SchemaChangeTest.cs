using HotChocolate.Execution;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using sport_sync.GraphQL.Mutations;
using sport_sync.GraphQL.Queries;

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
            .AddQueryType(q => q.Name("Query"))
            .AddType<UploadType>()
            .AddType<UserQuery>()
            .AddType<NotificationQuery>()
            .AddType<EventQuery>()
            .AddType<MatchQuery>()
            .AddType<FriendshipQuery>()
            .AddMutationType(q => q.Name("Mutation"))
            .AddType<UserMutation>()
            .AddType<NotificationMutation>()
            .AddType<MatchMutation>()
            .AddType<EventMutation>()
            .AddType<TeamMutation>()
            .AddType<FriendshipMutation>()
            .BuildSchemaAsync();

        schema.ToString().MatchSnapshot();
    }
}