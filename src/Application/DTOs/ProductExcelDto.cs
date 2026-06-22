using MiniExcelLibs.Attributes;

namespace Application.DTOs;

public class ProductExcelDto
{
    [ExcelColumn(Name = "Tên Sản Phẩm", Width = 30)]
    public string ProductName { get; set; } = string.Empty;

    [ExcelColumn(Name = "Danh Mục", Width = 15)]
    public string Category { get; set; } = string.Empty;

    [ExcelColumn(Name = "Giá Bán", Width = 15)]
    public decimal Price { get; set; }

    [ExcelColumn(Name = "Giá Gốc", Width = 15)]
    public decimal? OriginalPrice { get; set; }

    [ExcelColumn(Name = "Màu Sắc", Width = 15)]
    public string? Color { get; set; }

    [ExcelColumn(Name = "Size", Width = 10)]
    public string? Size { get; set; }

    [ExcelColumn(Name = "Tồn Kho", Width = 15)]
    public int StockQuantity { get; set; }

    [ExcelColumn(Name = "Link Ảnh", Width = 40)]
    public string? ImageUrl { get; set; }
}
