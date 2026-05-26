using System.Text.Json;

namespace Web.Helpers;

public static class RecentlyViewedManager
{
    private const string SessionKey = "TheOldPavementRecentlyViewedKey";
    private const int MaxItems = 4;

    public static List<int> GetRecentlyViewed(ISession session)
    {
        var json = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<int>();
        }
        try
        {
            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    public static void AddRecentlyViewed(ISession session, int productId)
    {
        var list = GetRecentlyViewed(session);
        // Remove duplicate if exists
        list.Remove(productId);
        // Insert at beginning
        list.Insert(0, productId);
        // Limit to max items
        if (list.Count > MaxItems)
        {
            list = list.Take(MaxItems).ToList();
        }
        session.SetString(SessionKey, JsonSerializer.Serialize(list));
    }
}
