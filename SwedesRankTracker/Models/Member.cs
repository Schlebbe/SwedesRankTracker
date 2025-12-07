using System;
using System.Collections.Generic;

namespace SwedesRankTracker.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string UserName { get; set; } = null!;

    public int Ehb { get; set; }

    public int Ehp { get; set; }

    public int Pets { get; set; }

    public int Collections { get; set; }

    public DateTime LastUpdated { get; set; }
}
