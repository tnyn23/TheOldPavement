using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
