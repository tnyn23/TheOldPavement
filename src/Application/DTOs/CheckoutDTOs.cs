using System.Collections.Generic;

namespace Application.DTOs;

public class CheckoutRequestDTO
{
    public int? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Note { get; set; }
    public string PaymentMethod { get; set; } = "cod";
    public int? PromoCodeId { get; set; }
    public List<CartItemDTO> CartItems { get; set; } = new();
    public decimal TotalPrice { get; set; }
}

public class CheckoutResultDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class PromoValidationResultDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? PromoId { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
}
