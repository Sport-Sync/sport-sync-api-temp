using SportSync.Api.Tests.Common;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Extensions;

public static class DatabaseExtensions
{
    public static User AddUser(
        this Database database,
        string firstName = "FirstName",
        string lastName = "LastName",
        string email = "test@gmail.com",
        string phone = "0986732423",
        string passwordHash = "nuir4gh4598gh")
    {
        var user = User.Create(firstName, lastName, email, phone, passwordHash);
        database.DbContext.Insert(user);

        return user;
    }

    public static User AddTermin(
        this Database database,
        string firstName = "FirstName",
        string lastName = "LastName",
        string email = "test@gmail.com",
        string phone = "0986732423",
        string passwordHash = "nuir4gh4598gh")
    {
        var user = User.termin(firstName, lastName, email, phone, passwordHash);
        database.DbContext.Insert(user);

        return user;
    }
}