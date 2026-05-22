namespace TheOldPavement.Application.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int DiscountPercentage { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsOnSale { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsOnSale { get; set; }
    public string Status { get; set; } = "available";
    public int StockQuantity { get; set; }
}

public class UpdateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsOnSale { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}
