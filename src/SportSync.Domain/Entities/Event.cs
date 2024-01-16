using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Event : AggregateRoot
{
    private readonly HashSet<EventMember> _members = new();
    private readonly HashSet<Termin> _termins = new();
    private readonly HashSet<EventSchedule> _schedules = new();

    private Event(User creator, string name, SportType sportType, string address, decimal price, int numberOfPlayers, string notes)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(creator, "The creator is required.", nameof(creator));
        Ensure.NotEmpty(creator.Id, "The creator identifier is required.", $"{nameof(creator)}{nameof(creator.Id)}");
        Ensure.NotEmpty(address, "The address is required.", $"{nameof(address)}");
        Ensure.NotEmpty(address, "The address is required.", $"{nameof(address)}");

        Name = name;
        SportType = sportType;
        Address = address;
        Price = price;
        NumberOfPlayers = numberOfPlayers;
        Notes = notes;

        _members.Add(EventMember.Create(creator.Id, Id, true));
    }

    private Event()
    {

    }

    public string Name { get; set; }
    public SportType SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayers { get; set; }
    public string Notes { get; set; }

    public IReadOnlyCollection<EventSchedule> Schedules => _schedules.ToList();
    public IReadOnlyCollection<EventMember> Members => _members.ToList();
    public IReadOnlyCollection<Termin> Termins => _termins.ToList();
    public List<Guid> MemberUserIds => _members.Select(m => m.UserId).ToList();

    public static Event Create(
        User creator, string name, SportType sportType, string address, decimal price, int numberOfPlayers, string notes)
    {
        var @event = new Event(creator, name, sportType, address, price, numberOfPlayers, notes);

        return @event;
    }

    public void AddSchedules(List<EventSchedule> schedules)
    {
        foreach (var schedule in schedules)
        {
            _schedules.Add(schedule);

            foreach (var termin in CreateTerminsBySchedule(schedule))
            {
                _termins.Add(termin);
            }
        }
    }

    public void AddMembers(List<Guid> userIds)
    {
        foreach (Guid userId in userIds)
        {
            if (_members.Any(x => x.UserId == userId))
            {
                continue;
            }
            _members.Add(EventMember.Create(userId, Id));
        }
    }

    private IEnumerable<Termin> CreateTerminsBySchedule(EventSchedule schedule)
    {
        var termin = Termin.Create(this, schedule.StartDate, schedule.StartTimeUtc, schedule.EndTimeUtc);

        termin.AddPlayers(MemberUserIds);

        yield return termin;

        if (schedule.RepeatWeekly)
        {
            int numberOfTerminsCreatedInFuture = 12; // TODO: Read from config
            var nextTerminDate = schedule.StartDate.AddDays(7);

            for (int i = 0; i < numberOfTerminsCreatedInFuture; i++)
            {
                var nextTermin = Termin.Create(this, nextTerminDate, schedule.StartTimeUtc, schedule.EndTimeUtc);

                nextTermin.AddPlayers(MemberUserIds);

                nextTerminDate = nextTerminDate.AddDays(7);

                yield return nextTermin;
            }
        }
    }
}