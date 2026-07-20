using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class State
{
    public int Id { get; set; }

    public string Abbreviation { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
