using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class Termin : Entity
{
    public Termin()
        : base(Guid.NewGuid())
    {
        
    }

    public Guid EventId { get; set; }
}