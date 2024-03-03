using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.ValueObjects;

public sealed class NotificationCommands : ValueObject
{
    public NotificationCommand[] Commands => Value.Split(",")
        .Select(x => (NotificationCommand)Enum.Parse(typeof(NotificationCommand), x))
        .ToArray();

    public string Value { get; }

    private NotificationCommands(NotificationCommand[] commands)
    {
        Value = string.Join(",", commands);
    }

    private NotificationCommands()
    {
    }

    public static NotificationCommands None => new();
    public static NotificationCommands AcceptReject => Create(NotificationCommand.Accept, NotificationCommand.Reject);
    public static NotificationCommands Create(params NotificationCommand[] commands) => new(commands);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Commands;
    }
}