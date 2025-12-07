using System;
using System.Collections.Generic;

namespace SwedesRankTracker.Models;

public partial class Rank
{
    public int RankId { get; set; }

    public string Name { get; set; } = null!;

    public int? MinEhp { get; set; }

    public int? MinEhb { get; set; }

    public int? MinTotalLevel { get; set; }

    public int? MinCollections { get; set; }

    public int? MinPets { get; set; }
}
