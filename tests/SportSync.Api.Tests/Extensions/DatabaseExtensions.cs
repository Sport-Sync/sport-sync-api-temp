using SportSync.Api.Tests.Common;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

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
        var user = User.Create(firstName, lastName, email, PhoneNumber.Create(phone).Value, passwordHash);
        database.DbContext.Insert(user);

        return user;
    }

    public static FriendshipRequest AddFriendshipRequest(
        this Database database,
        User user,
        User friend,
        bool accepted = false,
        bool rejected = false)
    {
        var friendshipRequest = new FriendshipRequest(user, friend);

        if (accepted)
        {
            friendshipRequest.Accept(DateTime.UtcNow);
        }

        if (rejected)
        {
            friendshipRequest.Reject(DateTime.UtcNow);
        }

        database.DbContext.Insert(friendshipRequest);

        return friendshipRequest;
    }

    public static Friendship AddFriendship(
        this Database database,
        User user,
        User friend)
    {
        var friendship = new Friendship(user, friend);
        database.DbContext.Insert(friendship);

        return friendship;
    }

    public static Termin AddTermin(
        this Database database,
        User user,
        string eventName = "event",
        SportType sportType = SportType.Football,
        EventSchedule schedule = null,
        DateTime startDate = default,
        TerminStatus status = TerminStatus.Pending)
    {
        var tomorrow = DateTime.Today.AddDays(1);
        schedule ??= EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), false);

        var ev = Event.Create(user, eventName, sportType, "address", 2, 10, null);
        var termin = Termin.Create(ev, startDate, schedule);
        termin.Status = status;

        database.DbContext.Set<EventSchedule>().Add(schedule);
        database.DbContext.Set<Event>().Add(ev);
        database.DbContext.Set<Termin>().Add(termin);

        var player = Player.Create(user.Id, termin.Id);

        database.DbContext.Set<Player>().Add(player);

        return termin;
    }

    public static TerminApplication AddTerminApplication(
        this Database database,
        User user,
        Termin termin)
    {
        var terminApplication = new TerminApplication(user, termin);

        database.DbContext.Set<TerminApplication>().Add(terminApplication);

        return terminApplication;
    }

    public static Notification AddNotification(
        this Database database,
        Guid userId,
        NotificationTypeEnum type = NotificationTypeEnum.FriendshipRequestReceived,
        Guid? resourceId = null)
    {
        var notification = Notification.Create(userId, type, resourceId);

        database.DbContext.Set<Notification>().Add(notification);

        return notification;
    }
}