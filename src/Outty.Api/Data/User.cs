using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public virtual Profile? Profile { get; set; }
}
