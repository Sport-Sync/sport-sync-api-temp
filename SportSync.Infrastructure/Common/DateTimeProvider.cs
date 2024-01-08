using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Infrastructure.Common;

internal sealed class MachineDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}