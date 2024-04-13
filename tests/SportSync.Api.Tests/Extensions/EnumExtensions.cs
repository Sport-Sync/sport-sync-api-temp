using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Extensions;

public static class EnumExtensions
{
    public static Error ToError(this MatchStatusEnum status)
    {
        if (status == MatchStatusEnum.Finished)
        {
            return DomainErrors.Match.AlreadyFinished;
        }

        if (status == MatchStatusEnum.Canceled)
        {
            return DomainErrors.Match.Canceled;
        }

        if (status == MatchStatusEnum.InProgress)
        {
            return DomainErrors.Match.InProgress;
        }

        return default;
    }
}