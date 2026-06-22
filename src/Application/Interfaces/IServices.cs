using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<ProductDTO?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDTO>> GetFeaturedProductsAsync();
    Task<IEnumerable<ProductDTO>> SearchProductsAsync(string keyword);
    Task<int> CreateProductAsync(CreateProductDTO dto);
    Task UpdateProductAsync(int id, UpdateProductDTO dto);
    Task DeleteProductAsync(int id);
    Task<SizeRecommendationResponseDto> GetSizeRecommendationAsync(SizeRecommendationRequestDto request);
    Task<byte[]> ExportProductsToExcelAsync(string webRootPath);
    Task ImportProductsFromExcelAsync(Stream excelStream, string webRootPath);
}

public interface IOrderService
{
    Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
    Task<OrderDTO?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId);
    Task<int> CreateOrderAsync(CreateOrderDTO dto);
    Task UpdateOrderStatusAsync(int id, string status);
    Task<bool> CancelOrderAsync(int orderId, int userId);
}

public interface IUserService
{
    Task<UserDTO?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<int> CreateUserAsync(CreateUserDTO dto);
    Task UpdateUserAsync(int id, UpdateUserDTO dto);
    Task DeleteUserAsync(int id);
}

public interface ICartService
{
    Task<CartDTO?> GetCartByUserIdAsync(int userId);
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task UpdateCartItemAsync(int cartItemId, int quantity);
    Task RemoveFromCartAsync(int cartItemId);
    Task ClearCartAsync(int userId);
}

public interface IPaymentService
{
    Task<string> CreatePaymentAsync(int orderId, string method);
    Task<bool> VerifyPaymentAsync(string paymentId);
}

public interface IAuthService
{
    Task<AuthResultDTO?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(CreateUserDTO dto);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    string GenerateToken(UserDTO user);
}

