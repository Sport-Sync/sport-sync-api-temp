using Newtonsoft.Json.Linq;

namespace SportSync.Api.Tests.Extensions;

public static class ResponseExtensions
{
    public static T ToObject<T>(this string graphQlJson, string rootNode)
    {
        var jObject = JObject.Parse(graphQlJson);
        var temp = jObject["data"][rootNode];

        return temp.ToObject<T>();
    }
}