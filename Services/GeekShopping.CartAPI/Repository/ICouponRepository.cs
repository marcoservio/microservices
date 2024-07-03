using GeekShopping.CartAPI.Data.ValueObjects;

namespace GeekShopping.CartAPI.Repository;

public interface ICouponRepository
{
    Task<CouponVO> GetByCouponCode(string couponCode, string token);
}
