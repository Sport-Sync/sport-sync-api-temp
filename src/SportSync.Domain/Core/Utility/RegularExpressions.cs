namespace SportSync.Domain.Core.Utility;

public static class RegularExpressions
{
    public const string Email = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
    public const string CroatianPhoneNumber = @"^(\+385\s?9[12589]\s?\d{3}\s?\d{3,4}|09[12589]\s?\d{3}\s?\d{3,4})$";
}