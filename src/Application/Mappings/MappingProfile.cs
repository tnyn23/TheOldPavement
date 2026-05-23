using AutoMapper;
using Application.DTOs;
using Domain.Models;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<CreateProductDTO, Product>();
        CreateMap<UpdateProductDTO, Product>();

        CreateMap<User, UserDTO>();
        CreateMap<CreateUserDTO, User>();
        CreateMap<UpdateUserDTO, User>();

        CreateMap<Order, OrderDTO>();
        CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
    }
}


