using AutoMapper;
using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<SizeChart> _sizeChartRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IRepository<SizeChart> sizeChartRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _sizeChartRepository = sizeChartRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }

    public async Task<ProductDTO?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;
        return _mapper.Map<ProductDTO>(product);
    }

    public async Task<IEnumerable<ProductDTO>> GetFeaturedProductsAsync()
    {
        var products = await _productRepository.GetFeaturedProductsAsync();
        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }

    public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string keyword)
    {
        var products = await _productRepository.SearchProductsAsync(keyword);
        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }

    public async Task<int> CreateProductAsync(CreateProductDTO dto)
    {
        var exists = await _productRepository.ExistsAsync(p => p.Name == dto.Name);
        if (exists)
        {
            throw new BusinessException(string.Format(ErrorMessages.AlreadyExists, "Product name"));
        }

        var product = _mapper.Map<Product>(dto);
        // generate slug if needed
        product.Slug = dto.Name.ToLower().Replace(" ", "-");
        
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        return product.Id;
    }

    public async Task UpdateProductAsync(int id, UpdateProductDTO dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException(string.Format(ErrorMessages.NotFound, "Product"));
        }

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException(string.Format(ErrorMessages.NotFound, "Product"));
        }

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task<SizeRecommendationResponseDto> GetSizeRecommendationAsync(SizeRecommendationRequestDto request)
    {
        var sizeCharts = await _sizeChartRepository.FindAsync(s => s.ProductCategory == request.ProductCategory);

        if (!sizeCharts.Any())
        {
            return new SizeRecommendationResponseDto { RecommendedSize = "", Message = "Không tìm thấy dữ liệu size cho danh mục này." };
        }

        var exactMatch = sizeCharts.FirstOrDefault(s => 
            s.MinHeight <= request.Height && s.MaxHeight >= request.Height &&
            s.MinWeight <= request.Weight && s.MaxWeight >= request.Weight);

        var bestMatch = exactMatch;

        if (bestMatch == null)
        {
            bestMatch = sizeCharts
                .OrderBy(s => Math.Abs((s.MinWeight ?? 0 + s.MaxWeight ?? 0) / 2.0 - request.Weight))
                .ThenBy(s => Math.Abs((s.MinHeight ?? 0 + s.MaxHeight ?? 0) / 2.0 - request.Height))
                .FirstOrDefault();
        }

        if (bestMatch == null)
        {
            return new SizeRecommendationResponseDto { RecommendedSize = "", Message = "Không thể tìm size phù hợp." };
        }

        var sortedSizes = sizeCharts.OrderBy(s => s.MinWeight ?? 0).ToList();
        int currentIndex = sortedSizes.IndexOf(bestMatch);
        int targetIndex = currentIndex;

        if (request.FitPreference == "Oversized")
        {
            targetIndex = Math.Min(currentIndex + 1, sortedSizes.Count - 1);
        }
        else if (request.FitPreference == "Fitted")
        {
            targetIndex = Math.Max(currentIndex - 1, 0);
        }

        var finalSize = sortedSizes[targetIndex];

        return new SizeRecommendationResponseDto
        {
            RecommendedSize = finalSize.Size,
            Message = $"Size {finalSize.Size} phù hợp với sở thích mặc {request.FitPreference} của bạn."
        };
    }

    public async Task<byte[]> ExportProductsToExcelAsync()
    {
        var products = await _productRepository.GetAllWithDetailsAsync();
        var records = new List<ProductExcelDto>();

        foreach (var p in products)
        {
            if (p.ProductVariants != null && p.ProductVariants.Any())
            {
                foreach (var v in p.ProductVariants)
                {
                    records.Add(new ProductExcelDto
                    {
                        ProductName = p.Name,
                        Category = p.Category,
                        Price = p.Price,
                        OriginalPrice = p.OriginalPrice,
                        Color = v.Color,
                        Size = v.Size,
                        StockQuantity = v.StockQuantity ?? 0
                    });
                }
            }
            else
            {
                // Product has no variants
                records.Add(new ProductExcelDto
                {
                    ProductName = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    Color = "",
                    Size = "",
                    StockQuantity = 0
                });
            }
        }

        using var memoryStream = new MemoryStream();
        await MiniExcelLibs.MiniExcel.SaveAsAsync(memoryStream, records);
        return memoryStream.ToArray();
    }

    public async Task ImportProductsFromExcelAsync(Stream excelStream)
    {
        var records = await MiniExcelLibs.MiniExcel.QueryAsync<ProductExcelDto>(excelStream);
        
        // Group by product name
        var grouped = records.GroupBy(r => r.ProductName?.Trim());

        foreach (var group in grouped)
        {
            var productName = group.Key;
            if (string.IsNullOrEmpty(productName)) continue;

            var firstRow = group.First();
            var slug = productName.ToLower().Replace(" ", "-").Replace("đ", "d").Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a").Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e").Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i").Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o").Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u").Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y").Replace("ă", "a").Replace("â", "a").Replace("ê", "e").Replace("ô", "o").Replace("ơ", "o").Replace("ư", "u");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

            var product = await _productRepository.GetBySlugAsync(slug);
            bool isNew = false;

            if (product == null)
            {
                // check if exists by name
                var existingProducts = await _productRepository.SearchProductsAsync(productName);
                product = existingProducts.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            }

            if (product == null)
            {
                isNew = true;
                product = new Product
                {
                    Name = productName,
                    Slug = slug,
                    Category = firstRow.Category ?? "tee",
                    Price = firstRow.Price,
                    OriginalPrice = firstRow.OriginalPrice,
                    Status = "available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            else
            {
                product.Price = firstRow.Price;
                product.OriginalPrice = firstRow.OriginalPrice;
                product.Category = firstRow.Category ?? product.Category;
                product.UpdatedAt = DateTime.UtcNow;
            }

            if (product.ProductVariants == null)
                product.ProductVariants = new List<ProductVariant>();

            foreach (var row in group)
            {
                // Match existing variant by color and size
                var colorStr = row.Color?.Trim()?.ToLower() ?? "";
                var sizeStr = row.Size?.Trim()?.ToUpper() ?? "";

                if (string.IsNullOrEmpty(colorStr) && string.IsNullOrEmpty(sizeStr))
                    continue; // Skip empty variants

                var existingVariant = product.ProductVariants.FirstOrDefault(v => 
                    (v.Color?.ToLower() == colorStr || string.IsNullOrEmpty(v.Color) && string.IsNullOrEmpty(colorStr)) &&
                    (v.Size?.ToUpper() == sizeStr || string.IsNullOrEmpty(v.Size) && string.IsNullOrEmpty(sizeStr)));

                if (existingVariant != null)
                {
                    existingVariant.StockQuantity = row.StockQuantity;
                }
                else
                {
                    product.ProductVariants.Add(new ProductVariant
                    {
                        Color = row.Color?.Trim(),
                        Size = row.Size?.Trim()?.ToUpper(),
                        StockQuantity = row.StockQuantity,
                        Sku = $"{slug}-{colorStr}-{sizeStr}".Replace("--", "-").TrimEnd('-'),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (isNew)
            {
                await _productRepository.AddAsync(product);
            }
            else
            {
                await _productRepository.UpdateAsync(product);
            }
        }

        await _productRepository.SaveChangesAsync();
    }
}


