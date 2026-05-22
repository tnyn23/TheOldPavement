using System;
using System.Collections.Generic;

namespace TheOldPavement.Domain.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? Material { get; set; }

    public string? Weight { get; set; }

    public string? Fit { get; set; }

    public string? Features { get; set; }

    public string? CareInstructions { get; set; }

    public string? MadeIn { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}

