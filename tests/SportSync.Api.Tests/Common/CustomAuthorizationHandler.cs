using HotChocolate.Authorization;
using HotChocolate.Resolvers;

namespace SportSync.Api.Tests.Common;

public class IntegrationTestAuthorizationHandler : IAuthorizationHandler
{
    public ValueTask<AuthorizeResult> AuthorizeAsync(IMiddlewareContext context, AuthorizeDirective directive,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.FromResult(AuthorizeResult.Allowed);
    }

    public ValueTask<AuthorizeResult> AuthorizeAsync(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.FromResult(AuthorizeResult.Allowed);
    }
}