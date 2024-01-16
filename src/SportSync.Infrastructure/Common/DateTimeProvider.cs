using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Infrastructure.Common;

internal sealed class DateTimeProvider : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}