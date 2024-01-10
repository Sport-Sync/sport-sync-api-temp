using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;

namespace SportSync.Infrastructure.Authentication;

internal sealed class UserIdentifierProvider : IUserIdentifierProvider
{
    public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")
                          ?? throw new ArgumentException("The user identifier claim is required.", nameof(httpContextAccessor));

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        UserId = userId;
    }

    public Guid UserId { get; }
}