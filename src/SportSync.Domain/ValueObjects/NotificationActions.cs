using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.ValueObjects;

public sealed class NotificationActions : ValueObject
{
    private const int ActionsLimit = 3;

    private string[] _actions { get; }
    public string[] Actions => _actions;
    public string Value { get; }

    private NotificationActions(string[] actions)
    {
        _actions = actions;
        Value = string.Join(",", actions);
    }

    private NotificationActions()
    {
    }

    public static NotificationActions None => new();
    public static NotificationActions Create(params string[] actions) => new(actions);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return _actions;
    }
}