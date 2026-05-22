using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    public int? DiscountPercentage { get; set; }

    public string Category { get; set; } = null!;

    public int? CollectionId { get; set; }

    public bool? IsCollab { get; set; }

    public string? CollabPartner { get; set; }

    public string? Status { get; set; }

    public string? Condition { get; set; }

    public string? OutletReason { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsOnSale { get; set; }

    public bool? IsOutlet { get; set; }

    public bool? IsLimitedEdition { get; set; }

    public int? LimitedQuantity { get; set; }

    public string? SerialNumberPrefix { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AddToCartEvent> AddToCartEvents { get; set; } = new List<AddToCartEvent>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<CollaborationProduct> CollaborationProducts { get; set; } = new List<CollaborationProduct>();

    public virtual Collection? Collection { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<ProductView> ProductViews { get; set; } = new List<ProductView>();

    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
