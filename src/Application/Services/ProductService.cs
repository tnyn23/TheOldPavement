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
}


