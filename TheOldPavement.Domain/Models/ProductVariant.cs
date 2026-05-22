using System;
using System.Collections.Generic;

namespace TheOldPavement.Domain.Models;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Size { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string? ColorHex { get; set; }

    public string Sku { get; set; } = null!;

    public int? StockQuantity { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AddToCartEvent> AddToCartEvents { get; set; } = new List<AddToCartEvent>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;
}

