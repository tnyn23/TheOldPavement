using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ProductView
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    public DateTime? ViewedAt { get; set; }

    public string? IpAddress { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User? User { get; set; }
}


