﻿using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.AnnounceMatch;

public class AnnounceMatchInput : IRequest<MatchType>
{
    public Guid MatchId { get; set; }
    public bool PublicAnnouncement { get; set; }
    public int PlayerLimit { get; set; }
    public string? Description { get; set; }
}