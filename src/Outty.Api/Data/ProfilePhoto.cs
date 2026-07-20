using System;
using System.Collections.Generic;

namespace Outty.Api.Data;

public partial class ProfilePhoto
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public string FileName { get; set; } = null!;

    public string BlobUrl { get; set; } = null!;

    public byte SortOrder { get; set; }

    public virtual Profile Profile { get; set; } = null!;
}
