using HotChocolate.Types.Descriptors;

namespace sport_sync.GraphQL;

public class EnumNamingConvention : DefaultNamingConventions
{
    public override string GetEnumValueName(object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        return value.ToString();
    }
}