using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.ValueObjects;

public class NotificationDetails : ValueObject
{
    public const string UserName = "UserName";
    public const string Termin = "Termin";

    private JObject _value;

    public string Value { get; private set; }

    private NotificationDetails()
    {
        _value = new JObject();
        Value = JsonConvert.SerializeObject(_value);
    }

    public NotificationDetails WithUserName(string userName)
    {
        _value[UserName] = userName;
        Value = JsonConvert.SerializeObject(_value);
        return this;
    }

    public NotificationDetails WithTermin(string termin)
    {
        _value[Termin] = termin;
        Value = JsonConvert.SerializeObject(_value);
        return this;
    }

    public static NotificationDetails Create()
    {
        return new NotificationDetails();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}