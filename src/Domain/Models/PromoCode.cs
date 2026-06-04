using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class PromoCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal Value { get; set; }

    public decimal? MinOrderValue { get; set; }

    public decimal? MaxDiscount { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("applies_to_category")]
    public string? AppliesToCategory { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.Column("applies_to_product_ids")]
    public string? AppliesToProductIds { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("required_quantity")]
    public int? RequiredQuantity { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("reward_quantity")]
    public int? RewardQuantity { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("required_user_tier")]
    public string? RequiredUserTier { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("is_combo")]
    public bool IsCombo { get; set; } = false;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
