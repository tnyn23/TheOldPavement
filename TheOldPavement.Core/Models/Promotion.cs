namespace TheOldPavement.Core.Models;

public class Promotion
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DiscountType { get; set; } = "percentage"; // percentage, fixed
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? MaxUsageCount { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
