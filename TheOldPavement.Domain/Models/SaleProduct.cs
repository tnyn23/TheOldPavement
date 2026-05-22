using System;
using System.Collections.Generic;

namespace TheOldPavement.Domain.Models;

public partial class SaleProduct
{
    public int Id { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int DiscountPercentage { get; set; }

    public decimal SalePrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;
}

