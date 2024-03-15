using SportSync.Domain.Core.Primitives.Maybe;

namespace SportSync.Application.Core.Abstractions.Common;

public interface IHttpHeaderProvider
{
    Maybe<string> Get(string key);
    Maybe<string> Language();
}