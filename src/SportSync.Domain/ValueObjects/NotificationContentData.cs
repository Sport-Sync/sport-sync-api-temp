using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.ValueObjects;

public class NotificationContentData : ValueObject
{
    public string Value { get; }
    public string[] Data => Value.Split(",");

    private NotificationContentData(params string[] contentData)
    {
        Value = contentData.Any() ? string.Join(",", contentData) : string.Empty;
    }


    private NotificationContentData()
    {
        Value = string.Empty;
    }

    public static NotificationContentData None => new ();

    public static NotificationContentData Create(params string[] contentData)
    {
        return new NotificationContentData(contentData);
    }

    public string this[int index] => Data.Length > index ? Data[index] : string.Empty;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value.Split(",");
    }
}