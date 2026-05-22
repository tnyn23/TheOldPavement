using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class UserAddress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Ward { get; set; }

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public bool? IsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
