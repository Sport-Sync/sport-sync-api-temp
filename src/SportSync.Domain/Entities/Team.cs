using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;

namespace SportSync.Domain.Entities;

public class Team : Entity, ISoftDeletableEntity
{
    private Team(string name, Guid eventId) 
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(name, "The name is required.", $"{nameof(name)}");

        Name = name;
        EventId = eventId;
    }

    private Team()
    {

    }

    public Guid EventId { get; set; }
    public string Name { get; set; }
    public DateTime? DeletedOnUtc { get; }
    public bool Deleted { get; }

    public static Team Create(string name, Guid eventId)
    {
        return new Team(name, eventId);
    }
}