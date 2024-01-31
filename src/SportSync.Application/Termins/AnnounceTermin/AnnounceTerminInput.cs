﻿using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Types;

namespace SportSync.Application.Termins.AnnounceTermin;

public class AnnounceTerminInput : IInput<TerminType>
{
    public Guid TerminId { get; set; }
    public bool PublicAnnouncement { get; set; }
}