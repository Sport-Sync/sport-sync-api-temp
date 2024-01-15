using HotChocolate;
using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class Event : AggregateRoot, IAuditableEntity, ISoftDeletableEntity
{
    [GraphQLIgnore]
    public DateTime CreatedOnUtc { get; set; }

    [GraphQLIgnore]
    public DateTime? ModifiedOnUtc { get; set; }

    [GraphQLIgnore]
    public DateTime? DeletedOnUtc { get; set; }

    [GraphQLIgnore]
    public bool Deleted { get; }
}