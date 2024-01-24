using Microsoft.Extensions.Logging;
using Moq;

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
}