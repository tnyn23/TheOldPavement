using Domain.Models;

namespace Application.Interfaces;

public interface IReviewService
{
    Task<bool> CanUserReviewProductAsync(int userId, int productId);
    Task<ProductReview> AddReviewAsync(int productId, int userId, int rating, string comment, string? title, List<string>? imageUrls);
    Task<bool> MarkHelpfulAsync(int reviewId);
    Task<List<ProductReview>> GetProductReviewsAsync(int productId, int? ratingFilter = null);
    
    // Admin methods
    Task<List<ProductReview>> GetAllReviewsAsync();
    Task<bool> ApproveReviewAsync(int reviewId);
    Task<bool> ToggleReviewApprovalAsync(int reviewId);
    Task<bool> DeleteReviewAsync(int reviewId);
}
