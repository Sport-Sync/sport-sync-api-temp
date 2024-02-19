using System.Text.RegularExpressions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;

namespace SportSync.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Lazy<Regex> PhoneNumberFormatRegex =
        new(() => new Regex(RegularExpressions.CroatianPhoneNumber, RegexOptions.Compiled | RegexOptions.IgnoreCase));

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value) =>
        Result.Create(value, DomainErrors.PhoneNumber.NullOrEmpty)
            .Ensure(e => !string.IsNullOrWhiteSpace(e), DomainErrors.PhoneNumber.NullOrEmpty)
            .Ensure(e => PhoneNumberFormatRegex.Value.IsMatch(e), DomainErrors.Email.InvalidFormat)
            .Map(e => new PhoneNumber(Beautify(value)));

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    private static string Beautify(string value) => value.Replace("+385", "0").Replace(" ", string.Empty);
}