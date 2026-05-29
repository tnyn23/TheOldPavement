using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int? UserId { get; set; }

    public string? Status { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public string? TransactionId { get; set; }

    public int? PromoCodeId { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual PromoCode? PromoCode { get; set; }

    public virtual ShippingAddress? ShippingAddress { get; set; }

    public virtual User? User { get; set; }
}


