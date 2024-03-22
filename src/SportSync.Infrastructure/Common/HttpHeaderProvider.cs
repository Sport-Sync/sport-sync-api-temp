using Microsoft.AspNetCore.Http;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Core.Primitives.Maybe;

namespace SportSync.Infrastructure.Common;

public class HttpHeaderProvider : IHttpHeaderProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpHeaderProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Maybe<string> Get(string key)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return Maybe<string>.None;
        }

        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(key, out var val))
        {
            return val.ToString();
        }

        return Maybe<string>.None;
    }
}