using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Web.Pages;

public class CollectionsModel : PageModel
{
    private readonly TheOldPavementDbContext _context;

    public List<Collection> CollectionsList { get; set; } = new();

    public CollectionsModel(TheOldPavementDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        // Seed Collections if table is empty
        if (!_context.Collections.Any())
        {
            var c1 = new Collection
            {
                Name = "BỘ SƯU TẬP HÀ NỘI RETRO 2026",
                Slug = "hanoi-retro",
                Season = "Autumn/Winter",
                Year = 2026,
                Description = "Bộ sưu tập lấy cảm hứng từ các nét văn hóa cổ kính, hình ảnh xích lô Hà Nội và nhịp sống phố cổ xưa cũ, lồng ghép tinh tế vào phong cách streetwear hiện đại.",
                HeroImageUrl = "https://images.unsplash.com/photo-1627225793904-a2f900a6e4cf?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var c2 = new Collection
            {
                Name = "TACTICAL UTILITY EDITION",
                Slug = "tactical-utility",
                Season = "Spring/Summer",
                Year = 2026,
                Description = "Những thiết kế mang tính thực dụng cao, tối ưu công năng sử dụng với các chất liệu chống thấm, nhiều túi hộp tiện lợi cùng tem nhãn thêu tay độc bản.",
                HeroImageUrl = "https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Collections.AddRange(c1, c2);
            await _context.SaveChangesAsync();

            // Link existing products to collections
            var products = await _context.Products.ToListAsync();
            foreach (var p in products)
            {
                if (p.Slug.Contains("stones") || p.Slug.Contains("36") || p.Slug.Contains("hanoi") || p.Slug.Contains("classic"))
                {
                    p.CollectionId = c1.Id;
                }
                else if (p.Slug.Contains("utility") || p.Slug.Contains("jacket") || p.Slug.Contains("commercial"))
                {
                    p.CollectionId = c2.Id;
                }
                else
                {
                    p.CollectionId = c1.Id; // Default
                }
                _context.Products.Update(p);
            }
            await _context.SaveChangesAsync();
        }

        CollectionsList = await _context.Collections
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }
}
