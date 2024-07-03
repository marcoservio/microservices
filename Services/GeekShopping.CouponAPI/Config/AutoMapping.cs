using AutoMapper;

using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Model;

namespace GeekShopping.CouponAPI.Config;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<CouponVO, Coupon>().ReverseMap();
    }
}
