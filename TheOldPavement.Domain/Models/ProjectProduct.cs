using System;
using System.Collections.Generic;

namespace TheOldPavement.Domain.Models;

public partial class ProjectProduct
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string? Sizes { get; set; }

    public string? Colors { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual CommercialProject Project { get; set; } = null!;
}

