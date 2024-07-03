using AutoMapper;

using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;

namespace GeekShopping.ProductAPI.Config;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<ProductVO, Product>();
        CreateMap<Product, ProductVO>();
    }
}
