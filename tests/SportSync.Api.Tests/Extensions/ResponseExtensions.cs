using FluentAssertions;
using Newtonsoft.Json.Linq;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Api.Tests.Extensions;

public static class ResponseExtensions
{
    public static T ToResponseObject<T>(this string graphQlJson, string rootNode)
    {
        var jObject = JObject.Parse(graphQlJson);
        var temp = jObject["data"][rootNode];

        jObject["errors"].Should().BeNull();

        return temp.ToObject<T>();
    }

    public static void ShouldBeSuccessResult(this string graphQlJson, string rootNode)
    {
        var jObject = JObject.Parse(graphQlJson);
        var temp = jObject["data"][rootNode];

        var result = temp.ToObject<ResultResponse>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Code.Should().BeNullOrEmpty();
        result.Error.Message.Should().BeNullOrEmpty();
    }

    public static void ShouldBeFailureResult(this string graphQlJson, string rootNode, Error error)
    {
        var jObject = JObject.Parse(graphQlJson);
        var temp = jObject["data"][rootNode];

        var result = temp.ToObject<ResultResponse>();
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(error.Code);
        result.Error.Message.Should().Be(error.Message);
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

public class ResultResponse
{
    public bool IsSuccess { get; set; }
    public bool IsFailure { get; set; }
    public ErrorDetails Error { get; set; }
}

public class ErrorDetails
{
    public string Code { get; set; }
    public string Message { get; set; }
}

public class ErrorResponse
{
    public List<GraphQLError> Errors { get; set; }
}

public class GraphQLError
{
    public string Message { get; set; }
}