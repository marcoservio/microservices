using AutoMapper;

using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Config;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<ProductVO, Product>().ReverseMap();
        CreateMap<CartHeaderVO, CartHeader>().ReverseMap();
        CreateMap<CartDetailVO, CartDetail>().ReverseMap();
        CreateMap<CartVO, Cart>().ReverseMap();
    }
}
