using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IRabbitMQMessageSender _rabbitMQMessageSender;

    public CartController(ICartRepository repository, IRabbitMQMessageSender rabbitMQMessageSender, ICouponRepository couponRepository)
    {
        _cartRepository = repository ?? throw new ArgumentException(null, nameof(repository));
        _rabbitMQMessageSender = rabbitMQMessageSender;
        _couponRepository = couponRepository;
    }

    [HttpGet("find-cart/{id}")]
    public async Task<IActionResult> FindByUserId(string id)
    {
        var cart = await _cartRepository.FindCartByUserId(id);

        if (cart == null)
            return NotFound();

        return Ok(cart);
    }

    [HttpPost("add-cart")]
    public async Task<IActionResult> Add(CartVO vo)
    {
        var cart = await _cartRepository.SaveOrUpdateCart(vo);

        if (cart == null)
            return NotFound();

        return Ok(cart);
    }

    [HttpPut("update-cart")]
    public async Task<IActionResult> Update(CartVO vo)
    {
        var cart = await _cartRepository.SaveOrUpdateCart(vo);

        if (cart == null)
            return NotFound();

        return Ok(cart);
    }

    [HttpDelete("remove-cart/{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var status = await _cartRepository.RemoveFromCart(id);

        if (!status)
            return BadRequest();

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<IActionResult> ApplyCoupon(CartVO vo)
    {
        if(string.IsNullOrWhiteSpace(vo.CartHeader?.UserId) || string.IsNullOrWhiteSpace(vo.CartHeader?.CuponCode))
            return NotFound();

        var status = await _cartRepository.ApplyCoupon(vo.CartHeader.UserId, vo.CartHeader.CuponCode);

        if (!status)
            return NotFound();

        return Ok(status);
    }

    [HttpDelete("remove-coupon/{userId}")]
    public async Task<IActionResult> RemoveCoupon(string userId)
    {
        var status = await _cartRepository.RemoveCoupon(userId);

        if (!status)
            return NotFound();

        return Ok(status);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutHeaderVO vo)
    {
        //string authorization = Request.Headers["Authorization"];
        //string token = authorization["Bearer ".Length..];
        var token = await HttpContext.GetTokenAsync("access_token");

        if (vo?.UserId == null)
            return BadRequest();

        var cart = await _cartRepository.FindCartByUserId(vo.UserId);

        if (cart == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(vo.CuponCode))
        {
            var coupon = await _couponRepository.GetByCouponCode(vo.CuponCode, token);

            if (vo.DiscountAmount != coupon.DiscountAmount)
                return StatusCode(412);
        }

        vo.CartDetails = cart.CartDetails;
        vo.DateTime = DateTime.UtcNow;

        _rabbitMQMessageSender.SendMessage(vo, "checkoutqueue");

        await _cartRepository.ClearCart(vo.UserId);

        return Ok(vo);
    }
}
