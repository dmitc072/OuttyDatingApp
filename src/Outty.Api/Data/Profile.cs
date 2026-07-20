using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class Profile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string DisplayName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string? Pronouns { get; set; }

    public string? Bio { get; set; }

    public string PreferredDistance { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual ICollection<ProfileInterest> ProfileInterests { get; set; } = new List<ProfileInterest>();

    public virtual ICollection<ProfilePhoto> ProfilePhotos { get; set; } = new List<ProfilePhoto>();

    public virtual State StateNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
