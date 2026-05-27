using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Web.Pages;

public class LookbookModel : PageModel
{
    private readonly TheOldPavementDbContext _context;

    public List<LookbookItem> Lookbooks { get; set; } = new();

    public LookbookModel(TheOldPavementDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        // Fetch products so we can reference real products in hotspots
        var products = await _context.Products.ToListAsync();
        var tee = products.FirstOrDefault(p => p.Slug.Contains("36") || p.Slug.Contains("retro")) ?? products.FirstOrDefault();
        var jacket = products.FirstOrDefault(p => p.Slug.Contains("utility") || p.Slug.Contains("jacket") || p.Slug.Contains("commercial")) ?? products.LastOrDefault();
        var blackTee = products.FirstOrDefault(p => p.Slug.Contains("classic-black")) ?? products.FirstOrDefault();

        // Build premium lookbook data with hotspots
        Lookbooks = new List<LookbookItem>
        {
            new()
            {
                Id = 1,
                Name = "HỒN PHỐ CỔ - AUTUMN/WINTER 2026",
                ImageUrl = "https://images.unsplash.com/photo-1627225793904-a2f900a6e4cf?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                Description = "Ghi lại nhịp điệu của lòng Hà Nội xưa cũ dưới góc nhìn thời trang đường phố đương đại.",
                Hotspots = new List<LookbookHotspot>
                {
                    new()
                    {
                        TopPercent = 45,
                        LeftPercent = 48,
                        ProductSlug = tee?.Slug ?? "36-pho-phuong",
                        ProductName = tee?.Name ?? "36 Phố Phường - Hồn Hà Nội Tee",
                        ProductPrice = tee?.Price ?? 425000
                    }
                }
            },
            new()
            {
                Id = 2,
                Name = "TACTICAL UTILITY LOOKS",
                ImageUrl = "https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                Description = "Sự tối giản trong cấu trúc thiết kế, nhấn mạnh vào chất liệu chống thấm và sự đa dụng.",
                Hotspots = new List<LookbookHotspot>
                {
                    new()
                    {
                        TopPercent = 35,
                        LeftPercent = 55,
                        ProductSlug = jacket?.Slug ?? "commercial-utility-jacket",
                        ProductName = jacket?.Name ?? "OP Tactical Utility Jacket",
                        ProductPrice = jacket?.Price ?? 899000
                    },
                    new()
                    {
                        TopPercent = 65,
                        LeftPercent = 50,
                        ProductSlug = blackTee?.Slug ?? "classic-black-tee",
                        ProductName = blackTee?.Name ?? "Classic Black Tee",
                        ProductPrice = blackTee?.Price ?? 425000
                    }
                }
            }
        };
    }
}

public class LookbookItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<LookbookHotspot> Hotspots { get; set; } = new();
}

public class LookbookHotspot
{
    public double TopPercent { get; set; }
    public double LeftPercent { get; set; }
    public string ProductSlug { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
}
