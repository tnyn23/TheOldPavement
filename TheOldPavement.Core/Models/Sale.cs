using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class Sale
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public string? BannerImageUrl { get; set; }

    public string? TermsAndConditions { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();
}
