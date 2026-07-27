using System;

namespace Outty.Api.Data;

public partial class Swipe
{
    public int Id { get; set; }

    public int SwiperProfileId { get; set; }

    public int TargetProfileId { get; set; }

    public bool Liked { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public virtual Profile SwiperProfile { get; set; } = null!;

    public virtual Profile TargetProfile { get; set; } = null!;
}
