using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class Collection
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? Season { get; set; }

    public int? Year { get; set; }

    public bool? IsActive { get; set; }

    public string? HeroImageUrl { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
