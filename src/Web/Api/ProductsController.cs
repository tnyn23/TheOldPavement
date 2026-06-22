using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("recommend-size")]
    public async Task<IActionResult> RecommendSize([FromBody] SizeRecommendationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _productService.GetSizeRecommendationAsync(request);
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportProducts()
    {
        try
        {
            var fileContents = await _productService.ExportProductsToExcelAsync();
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportProducts(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Vui lòng chọn một file Excel hợp lệ." });
        }

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Chỉ hỗ trợ định dạng file .xlsx." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            await _productService.ImportProductsFromExcelAsync(stream);
            return Ok(new { message = "Nhập dữ liệu thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Lỗi khi nhập dữ liệu: {ex.Message}" });
        }
    }
}
