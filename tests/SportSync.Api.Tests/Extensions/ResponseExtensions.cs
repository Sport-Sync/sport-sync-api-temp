using FluentAssertions;
using Newtonsoft.Json.Linq;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Api.Tests.Extensions;

public static class ResponseExtensions
{
    public static T ToObject<T>(this string graphQlJson, string rootNode)
    {
        var jObject = JObject.Parse(graphQlJson);
        var temp = jObject["data"][rootNode];

        jObject["errors"].Should().BeNull();

        return temp.ToObject<T>();
    }

    public static void ShouldHaveError(this string graphQlJson, params string[] errors)
    {
        var jObject = JObject.Parse(graphQlJson);

        var err = jObject.ToObject<ErrorResponse>().Errors;

        foreach (var error in errors)
        {
            err.Should().Contain(x => x.Message == error);
        }
    }

    public static void ShouldHaveError(this string graphQlJson, Error error)
    {
        var jObject = JObject.Parse(graphQlJson);

        var err = jObject.ToObject<ErrorResponse>().Errors;
        err.Should().Contain(x => x.Message == error.Message);
    }
}

public class ErrorResponse
{
    public List<GraphQLError> Errors { get; set; }
}

public class GraphQLError
{
    public string Message { get; set; }
}