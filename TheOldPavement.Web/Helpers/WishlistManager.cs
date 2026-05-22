using System.Text.Json;
using TheOldPavement.Application.DTOs;

namespace TheOldPavement.Web.Helpers;

public static class WishlistManager
{
    private const string WishlistSessionKey = "TheOldPavementWishlistKey";

    public static List<WishlistItemDTO> GetWishlist(ISession session)
    {
        var json = session.GetString(WishlistSessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<WishlistItemDTO>();
        }
        try
        {
            return JsonSerializer.Deserialize<List<WishlistItemDTO>>(json) ?? new List<WishlistItemDTO>();
        }
        catch
        {
            return new List<WishlistItemDTO>();
        }
    }

    public static void SaveWishlist(ISession session, List<WishlistItemDTO> wishlist)
    {
        var json = JsonSerializer.Serialize(wishlist);
        session.SetString(WishlistSessionKey, json);
    }

    public static void ToggleWishlist(ISession session, WishlistItemDTO item)
    {
        var wishlist = GetWishlist(session);
        var existing = wishlist.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existing != null)
        {
            wishlist.Remove(existing);
        }
        else
        {
            wishlist.Add(item);
        }

        SaveWishlist(session, wishlist);
    }

    public static bool IsInWishlist(ISession session, int productId)
    {
        return GetWishlist(session).Any(i => i.ProductId == productId);
    }

    public static int GetTotalItems(ISession session)
    {
        return GetWishlist(session).Count;
    }
}

public class WishlistItemDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
