using System.Text.Json;
using Application.DTOs;

namespace Web.Helpers;

public static class CartManager
{
    private const string CartSessionKey = "TheOldPavementCartKey";

    public static List<CartItemDTO> GetCart(ISession session)
    {
        var cartJson = session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItemDTO>();
        }
        try
        {
            return JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson) ?? new List<CartItemDTO>();
        }
        catch
        {
            return new List<CartItemDTO>();
        }
    }

    public static void SaveCart(ISession session, List<CartItemDTO> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        session.SetString(CartSessionKey, cartJson);
    }

    public static void AddToCart(ISession session, CartItemDTO item)
    {
        var cart = GetCart(session);
        var existingItem = cart.FirstOrDefault(i => i.ProductId == item.ProductId && i.Size == item.Size && i.Color == item.Color);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            // Assign a local ID for easier manipulation in UI
            item.Id = cart.Count > 0 ? cart.Max(i => i.Id) + 1 : 1;
            cart.Add(item);
        }

        SaveCart(session, cart);
    }

    public static void UpdateQuantity(ISession session, int itemId, int quantity)
    {
        if (quantity <= 0)
        {
            RemoveFromCart(session, itemId);
            return;
        }

        var cart = GetCart(session);
        var item = cart.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.Quantity = quantity;
            SaveCart(session, cart);
        }
    }

    public static void RemoveFromCart(ISession session, int itemId)
    {
        var cart = GetCart(session);
        var item = cart.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            cart.Remove(item);
            SaveCart(session, cart);
        }
    }

    public static void ClearCart(ISession session)
    {
        session.Remove(CartSessionKey);
    }

    public static int GetTotalItems(ISession session)
    {
        return GetCart(session).Sum(i => i.Quantity);
    }

    public static decimal GetTotalPrice(ISession session)
    {
        return GetCart(session).Sum(i => i.TotalPrice);
    }
}

