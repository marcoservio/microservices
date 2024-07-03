using GeekShopping.CouponAPI.Repository;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CouponController : ControllerBase
{
    private readonly ICouponRepository _repository;

    public CouponController(ICouponRepository repository)
    {
        _repository = repository ?? throw new ArgumentException(null, nameof(repository));
    }

    [HttpGet("{couponCode}")]
    [Authorize]
    public async Task<IActionResult> FindById(string couponCode)
    {
        var coupon = await _repository.GetByCouponCode(couponCode);

        if (coupon == null)
            return NotFound();

        return Ok(coupon);
    }
}
