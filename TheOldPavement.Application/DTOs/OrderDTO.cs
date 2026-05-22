namespace TheOldPavement.Application.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDTO> Items { get; set; } = new();
}

public class OrderItemDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateOrderDTO
{
    public int UserId { get; set; }
    public string PaymentMethod { get; set; } = "cod";
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? PromoCode { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
}

public class CreateOrderItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
