using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class Store
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? OpeningHours { get; set; }

    public string? GoogleMapsUrl { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
