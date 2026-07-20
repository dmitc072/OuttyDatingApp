using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class ProfileInterest
{
    public int ProfileId { get; set; }

    public byte InterestId { get; set; }

    public byte ExperienceLevelId { get; set; }

    public virtual ExperienceLevel ExperienceLevel { get; set; } = null!;

    public virtual Interest Interest { get; set; } = null!;

    public virtual Profile Profile { get; set; } = null!;
}
