using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Domain.Entities;

public class Event : AggregateRoot
{
    private readonly HashSet<EventMember> _members = new();
    private readonly HashSet<EventSchedule> _schedules = new();
    private readonly HashSet<EventInvitation> _invitations = new();

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
    public EventStatus Status { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayers { get; set; }
    public string Notes { get; set; }

    public IReadOnlyCollection<EventSchedule> Schedules => _schedules.ToList();
    public IReadOnlyCollection<EventInvitation> Invitations => _invitations.ToList();
    public IReadOnlyCollection<EventMember> Members => _members.ToList();
    public IReadOnlyCollection<EventMember> Admins => _members.Where(m => m.IsAdmin).ToList();
    public List<Guid> MemberUserIds => _members.Select(m => m.UserId).ToList();

    public static Event Create(
        User creator, string name, SportType sportType, string address, decimal price, int numberOfPlayers, string notes)
    {
        var @event = new Event(creator, name, sportType, address, price, numberOfPlayers, notes);

        @event.RaiseDomainEvent(new EventCreatedDomainEvent(@event));

        return @event;
    }

    public Result AcceptInvitation(Guid invitationId, User user, DateTime utcNow)
    {
        var invitation = _invitations.FirstOrDefault(i => i.Id == invitationId);

        if (invitation == null)
        {
            return Result.Failure(DomainErrors.EventInvitation.NotFound);
        }

        if (invitation.SentToUserId != user.Id)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        var result = invitation.Accept(utcNow);

        if (result.IsFailure)
        {
            return result;
        }

        AddMembers(user.Id);

        RaiseDomainEvent(new EventInvitationAcceptedDomainEvent(invitation, user, this));

        return Result.Success();
    }

    public Result RejectInvitation(Guid invitationId, User user, DateTime utcNow)
    {
        var invitation = _invitations.FirstOrDefault(i => i.Id == invitationId);

        if (invitation == null)
        {
            return Result.Failure(DomainErrors.EventInvitation.NotFound);
        }

        if (invitation.SentToUserId != user.Id)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        var result = invitation.Reject(utcNow);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(new EventInvitationRejectedDomainEvent(invitation, user, this));

        return Result.Success();
    }

    public void AddMembers(params Guid[] userIds)
    {
        foreach (Guid userId in userIds)
        {
            if (!IsMember(userId))
            {
                _members.Add(EventMember.Create(userId, Id));
            }
        }
    }

    public async Task<Result<EventInvitation>> InviteUser(User invitationSender, User invitationReceiver, IEventRepository eventRepository)
    {
        if (!IsAdmin(invitationSender.Id))
        {
            return Result.Failure<EventInvitation>(DomainErrors.User.Forbidden);
        }

        if (IsMember(invitationReceiver.Id))
        {
            return Result.Failure<EventInvitation>(DomainErrors.EventInvitation.AlreadyMember);
        }

        var existingInvitations = await eventRepository.GetPendingInvitations(Id, CancellationToken.None);

        if (existingInvitations.Any(i => i.SentToUserId == invitationReceiver.Id))
        {
            return Result.Failure<EventInvitation>(DomainErrors.EventInvitation.AlreadyInvited);
        }

        var invitation = EventInvitation.Create(invitationSender, invitationReceiver, this);
        _invitations.Add(invitation);

        RaiseDomainEvent(new EventInvitationSentDomainEvent(invitation, this));

        return invitation;
    }

    public Result MakeAdmin(User user)
    {
        if (!IsMember(user.Id))
        {
            return Result.Failure(DomainErrors.Event.NotMember);
        }

        if (IsAdmin(user.Id))
        {
            return Result.Success();
        }

        var member = _members.Single(x => x.UserId == user.Id);
        member.IsAdmin = true;

        return Result.Success();
    }

    public void AddSchedules(List<EventSchedule> schedules)
    {
        foreach (var schedule in schedules)
        {
            _schedules.Add(schedule);
        }
    }

    public bool IsMember(Guid userId)
    {
        return _members.Any(x => x.UserId == userId);
    }

    public bool IsAdmin(Guid userId)
    {
        return _members.Any(x => x.UserId == userId && x.IsAdmin);
    }
}