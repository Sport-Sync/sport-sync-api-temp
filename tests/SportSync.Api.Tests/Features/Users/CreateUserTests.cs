using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SportSync.Application.Authentication;
using SportSync.GraphQL;

namespace SportSync.Api.Tests.Features.Users;

public class CreateUserTests
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
            .AddMutationType<Mutation>()
            .ExecuteRequestAsync("createUser(input: {firstName: \"Marko\", lastName: \"Zdravko\", email: \"marin@gmail.com\",phone: \"0916\", password: \"mypass1.23MM\"}){token}");

        var json = schema.ToJson();

        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
    }
}