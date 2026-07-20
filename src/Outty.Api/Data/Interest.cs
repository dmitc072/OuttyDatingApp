using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class Interest
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProfileInterest> ProfileInterests { get; set; } = new List<ProfileInterest>();
}
