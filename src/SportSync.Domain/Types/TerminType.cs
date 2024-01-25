﻿using System.Linq.Expressions;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Types;

public class TerminType
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTimeUtc { get; set; }
    public TimeOnly EndTimeUtc { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public IEnumerable<PlayerType> Players { get; set; }
    public string Notes { get; set; }

    public static Expression<Func<Termin, TerminType>> PropertySelector = x => new TerminType
    {
        Id = x.Id,
        Address = x.Address,
        Date = x.Date,
        EndTimeUtc = x.EndTimeUtc,
        StartTimeUtc = x.StartTimeUtc,
        SportType = x.SportType,
        NumberOfPlayersExpected = x.NumberOfPlayersExpected,
        Price = x.Price,
        Notes = x.Notes,
        EventName = x.EventName,
        Players = x.Players.Select(p => new PlayerType
        {
            UserId = p.UserId,
            FirstName = p.User.FirstName,
            LastName = p.User.LastName,
            IsAttending = p.Attending
        })
    };
}

public class PlayerType
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsAttending { get; set; }
}