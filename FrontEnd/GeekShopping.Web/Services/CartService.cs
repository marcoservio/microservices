using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Utils;

using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services;

public class CartService : ICartService
{
    public const string BasePath = "api/v1/cart";
    private readonly HttpClient _client;

    public CartService(HttpClient client)
    {
        _client = client ?? throw new ArgumentException(null, nameof(client));
    }

    public async Task<CartViewModel> FindCartByUserId(string userId, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"{BasePath}/find-cart/{userId}");
        return await response.ReadContentAsync<CartViewModel>();
    }

    public async Task<CartViewModel> AddItemToCart(CartViewModel model, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync($"{BasePath}/add-cart", model);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<CartViewModel>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<CartViewModel> UpdateToCart(CartViewModel model, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync($"{BasePath}/update-cart", model);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<CartViewModel>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> RemoveFromCart(long cartId, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"{BasePath}/remove-cart/{cartId}");

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<bool>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> RemoveCoupon(string userId, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"{BasePath}/remove-coupon/{userId}");

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<bool>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> ApplyCoupon(CartViewModel cart, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync($"{BasePath}/apply-coupon", cart);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<bool>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<object> Checkout(CartHeaderViewModel cartHeader, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync($"{BasePath}/checkout", cartHeader);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<CartHeaderViewModel>();
        else if (response.StatusCode == HttpStatusCode.PreconditionFailed)
            return "Coupon Price has changed, please confirm!";

        throw new Exception("Something went wrong when calling API");
    }

    public Task<bool> ClearCart(string userId, string token)
    {
        throw new NotImplementedException();
    }
}
