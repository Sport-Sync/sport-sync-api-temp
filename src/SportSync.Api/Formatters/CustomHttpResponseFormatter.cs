using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;
using SportSync.Domain.Core.Exceptions;

namespace sport_sync.Formatters;

public class CustomHttpResponseFormatter : DefaultHttpResponseFormatter
{
    protected override HttpStatusCode OnDetermineStatusCode(
        IQueryResult result, FormatInfo format,
        HttpStatusCode? proposedStatusCode)
    {
        var possibleError = result.Errors?.FirstOrDefault(x => x.Exception is DomainException);

        if (possibleError != null)
        {
            return (possibleError.Exception as DomainException)!.Error.Code.StartsWith("Authentication") ?
                HttpStatusCode.Unauthorized : HttpStatusCode.BadRequest;
        }
        
        if (result.Errors?.Count > 0)
        {
            return HttpStatusCode.BadRequest;
        }
        
        return base.OnDetermineStatusCode(result, format, proposedStatusCode);
    }
}