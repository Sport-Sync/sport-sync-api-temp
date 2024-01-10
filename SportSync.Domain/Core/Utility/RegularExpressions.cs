namespace SportSync.Domain.Core.Utility;

public static class RegularExpressions
{
    public const string Email = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
    public const string CroatianPhoneNumber = @"^(\+3859[1258]\d{6,7}|09[1258]\d{6,7})$";
}