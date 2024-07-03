namespace GeekShopping.CartAPI.Data.ValueObjects;

public class CartHeaderVO
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CuponCode { get; set; } = string.Empty;
}
