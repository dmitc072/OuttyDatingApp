using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class ExperienceLevel
{
    public byte Id { get; set; }

    public string ExperienceLevel1 { get; set; } = null!;

    public virtual ICollection<ProfileInterest> ProfileInterests { get; set; } = new List<ProfileInterest>();
}
