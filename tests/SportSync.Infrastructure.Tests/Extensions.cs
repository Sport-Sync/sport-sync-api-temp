using Microsoft.Extensions.Logging;
using Moq;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Enumerations;

namespace SportSync.Infrastructure.Tests;

public static class Extensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message, Times timesCalled)
    {
        loggerMock.Verify(l => l.Log(
            logLevel,
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == message && @type.Name == "FormattedLogValues"),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), timesCalled);
    }

    public static string ToError(this MatchStatusEnum status)
    {
        if (status == MatchStatusEnum.Finished)
        {
            return DomainErrors.Match.AlreadyFinished.Message;
        }

        if (status == MatchStatusEnum.Canceled)
        {
            return DomainErrors.Match.Canceled.Message;
        }

        if (status == MatchStatusEnum.InProgress)
        {
            return DomainErrors.Match.InProgress.Message;
        }

        return default;
    }
}