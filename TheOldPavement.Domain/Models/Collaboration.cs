using System;
using System.Collections.Generic;

namespace TheOldPavement.Domain.Models;

public partial class Collaboration
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Tagline { get; set; }

    public string? Description { get; set; }

    public string? PartnerName { get; set; }

    public int? Year { get; set; }

    public string? Status { get; set; }

    public bool? IsFeatured { get; set; }

    public string? BannerImageUrl { get; set; }

    public string? DetailImages { get; set; }

    public string? AuthenticityInfo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CollaborationProduct> CollaborationProducts { get; set; } = new List<CollaborationProduct>();
}

