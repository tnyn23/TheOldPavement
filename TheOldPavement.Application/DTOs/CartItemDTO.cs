namespace TheOldPavement.Application.DTOs;

public class CartDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItemDTO> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    public int TotalItems => Items.Sum(i => i.Quantity);
}

public class CartItemDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public string? ProductThumbnail { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
