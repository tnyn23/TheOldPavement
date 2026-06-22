using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class SizeRecommendationRequestDto
{
    [Required]
    [Range(140, 200, ErrorMessage = "Chiều cao phải từ 140cm đến 200cm")]
    public int Height { get; set; }

    [Required]
    [Range(40, 120, ErrorMessage = "Cân nặng phải từ 40kg đến 120kg")]
    public int Weight { get; set; }

    [Required]
    public string FitPreference { get; set; } = "Regular"; // Fitted, Regular, Oversized

    [Required]
    public string ProductCategory { get; set; } = null!;
}

public class SizeRecommendationResponseDto
{
    public string RecommendedSize { get; set; } = null!;
    public string Message { get; set; } = null!;
}
