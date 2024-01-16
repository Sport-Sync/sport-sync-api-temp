﻿using System.ComponentModel.DataAnnotations.Schema;
using HotChocolate;
using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Entities;

public class Event : AggregateRoot, IAuditableEntity, ISoftDeletableEntity
{
    private readonly HashSet<EventMember> _members = new();
    private readonly Guid _creatorId;

    internal Event(User creator, string name, SportType sportType, string address, decimal price, int numberOfPlayers, IEnumerable<EventTime> schedule, string notes)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(creator, "The creator is required.", nameof(creator));
        Ensure.NotEmpty(creator.Id, "The creator identifier is required.", $"{nameof(creator)}{nameof(creator.Id)}");
        Ensure.NotEmpty(address, "The address is required.", $"{nameof(address)}");
        Ensure.NotEmpty(address, "The address is required.", $"{nameof(address)}");

        _creatorId = creator.Id;
        Name = name;
        SportType = sportType;
        Address = address;
        Price = price;
        NumberOfPlayers = numberOfPlayers;
        Notes = notes;
        Schedule = string.Join("; ", schedule.Select(x => x.ToString()));

        _members.Add(EventMember.Create(_creatorId, Id, true));
        CreateFutureTermins();
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
    public string Schedule { get; set; }

    [NotMapped]
    public Guid CreatorId => _creatorId;
    public IReadOnlyCollection<EventMember> Members => _members.ToList();

    [GraphQLIgnore]
    public DateTime CreatedOnUtc { get; set; }

    [GraphQLIgnore]
    public DateTime? ModifiedOnUtc { get; set; }

    [GraphQLIgnore]
    public DateTime? DeletedOnUtc { get; set; }

    [GraphQLIgnore]
    public bool Deleted { get; }

    public void AddMember(Guid userId)
    {
        _members.Add(EventMember.Create(userId, this.Id));
    }

    private void CreateFutureTermins()
    {

    }
}