namespace GeekShopping.Web.Models;

public class CartHeaderViewModel
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CuponCode { get; set; } = string.Empty;
    public decimal PurchaseAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public string ExpiryMothYear { get; set; } = string.Empty;
}
