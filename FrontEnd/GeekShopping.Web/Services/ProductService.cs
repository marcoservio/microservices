using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Utils;

using Microsoft.AspNetCore.Mvc;

using System.Net.Http.Headers;

namespace GeekShopping.Web.Services;

public class ProductService : IProductService
{
    public const string BasePath = "api/v1/product";
    private readonly HttpClient _client;

    public ProductService(HttpClient client)
    {
        _client = client ?? throw new ArgumentException(nameof(client));
    }

    public async Task<IEnumerable<ProductViewModel>> FindAll(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync(BasePath);
        return await response.ReadContentAsync<List<ProductViewModel>>();
    }

    public async Task<ProductViewModel> FindById(long id, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"{BasePath}/{id}");
        return await response.ReadContentAsync<ProductViewModel>();
    }

    public async Task<ProductViewModel> Create([FromBody] ProductViewModel model, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync(BasePath, model);

        if(response.IsSuccessStatusCode)
            return await response.ReadContentAsync<ProductViewModel>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<ProductViewModel> Update([FromBody] ProductViewModel model, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync(BasePath, model);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<ProductViewModel>();

        throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> DeleteById(long id, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"{BasePath}/{id}");
        
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAsync<bool>();

        throw new Exception("Something went wrong when calling API");
    }
}
