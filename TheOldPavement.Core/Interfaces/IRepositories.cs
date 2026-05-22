using TheOldPavement.Core.Models;

namespace TheOldPavement.Core.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> SearchProductsAsync(string keyword);
    Task<IEnumerable<Product>> GetByStatusAsync(string status);
    Task<Product?> GetBySlugAsync(string slug);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
    Task<Order?> GetOrderWithItemsAsync(int orderId);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetCartByUserIdAsync(int userId);
    Task<Cart?> GetCartWithItemsAsync(int userId);
}
