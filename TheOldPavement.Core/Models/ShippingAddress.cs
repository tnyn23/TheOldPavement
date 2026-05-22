using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class ShippingAddress
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Ward { get; set; }

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
