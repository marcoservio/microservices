using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Utils;

using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services;

public class CouponService : ICouponService
{
    public const string BasePath = "api/v1/coupon";
    private readonly HttpClient _client;

    public CouponService(HttpClient client)
    {
        _client = client ?? throw new ArgumentException(null, nameof(client));
    }

    public async Task<CouponViewModel> GetCoupon(string code, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"{BasePath}/{code}");

        if (response.StatusCode != HttpStatusCode.OK)
            return new CouponViewModel();

        return await response.ReadContentAsync<CouponViewModel>();
    }
}
