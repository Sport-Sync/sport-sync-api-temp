using HotChocolate;
using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Services;

namespace SportSync.Domain.Entities;

public class User : Entity, IAuditableEntity, ISoftDeletableEntity
{
    private string _passwordHash;

    private User(string firstName, string lastName, string email, string phone, string passwordHash)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(firstName, "The first name is required.", nameof(firstName));
        Ensure.NotEmpty(lastName, "The last name is required.", nameof(lastName));
        Ensure.NotEmpty(email, "The email is required.", nameof(email));
        Ensure.NotEmpty(phone, "The phone is required.", nameof(phone));
        Ensure.NotEmpty(passwordHash, "The password hash is required", nameof(passwordHash));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        _passwordHash = passwordHash;
    }

    private User()
    {
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [GraphQLIgnore]
    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; }

    public string Phone { get; set; }

    [GraphQLIgnore]
    public DateTime CreatedOnUtc { get; set; }

    [GraphQLIgnore]
    public DateTime? ModifiedOnUtc { get; set; }

    [GraphQLIgnore]
    public DateTime? DeletedOnUtc { get; set; }

    [GraphQLIgnore]
    public bool Deleted { get; }

    public Result ChangePassword(string passwordHash)
    {
        if (passwordHash == _passwordHash)
        {
            return Result.Failure(DomainErrors.User.CannotChangePassword);
        }

        _passwordHash = passwordHash;

        return Result.Success();
    }

    [GraphQLIgnore]
    public bool VerifyPasswordHash(string password, IPasswordHashChecker passwordHashChecker)
        => !string.IsNullOrWhiteSpace(password) && passwordHashChecker.HashesMatch(_passwordHash, password);

    public static User Create(string firstName, string lastName, string email, string phone, string passwordHash)
    {
        return new User(firstName, lastName, email, phone, passwordHash);
    }
}