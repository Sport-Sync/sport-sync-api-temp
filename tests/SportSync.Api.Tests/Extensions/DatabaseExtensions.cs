using SportSync.Api.Tests.Common;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

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

    public static Termin AddTermin(
        this Database database,
        User user,
        string eventName = "event",
        SportType sportType = SportType.Football,
        EventSchedule schedule = null,
        DateOnly startDate = default)
    {
        schedule ??= EventSchedule.Create(DayOfWeek.Wednesday, new DateOnly(2024, 1, 1), TimeOnly.MinValue, TimeOnly.MaxValue, false);

        var ev = Event.Create(user, eventName, sportType, "address", 2, 10, null);
        var termin = Termin.Create(ev, startDate, schedule);
        
        database.DbContext.Set<Termin>().Add(termin);

        var player = Player.Create(user.Id, termin.Id);

        database.DbContext.Set<Player>().Add(player);

        return termin;
    }
}