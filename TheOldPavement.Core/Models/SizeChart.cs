using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class SizeChart
{
    public int Id { get; set; }

    public string ProductCategory { get; set; } = null!;

    public string Size { get; set; } = null!;

    public string? Chest { get; set; }

    public string? Length { get; set; }

    public string? Shoulder { get; set; }

    public string? Sleeve { get; set; }

    public DateTime? CreatedAt { get; set; }
}
