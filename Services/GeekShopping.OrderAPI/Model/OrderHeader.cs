using GeekShopping.OrderAPI.Model.Base;

using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.OrderAPI.Model;

[Table("order_header")]
public class OrderHeader : BaseEntity
{
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;
    [Column("cupon_code")]
    public string CouponCode { get; set; } = string.Empty;
    [Column("purchase_amount")]
    public decimal PurchaseAmount { get; set; }
    [Column("discount_amount")]
    public decimal DiscountAmount { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;
    [Column("purchase_date")]
    public DateTime DateTime { get; set; }
    [Column("orderTime_time")]
    public DateTime OrderTime { get; set; }
    [Column("phone_number")]
    public string Phone { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("card_number")]
    public string CardNumber { get; set; } = string.Empty;
    [Column("cvv")]
    public string CVV { get; set; } = string.Empty;
    [Column("expiry_month_year")]
    public string ExpiryMonthYear { get; set; } = string.Empty;
    [Column("total_itens")]
    public int CartTotalItens { get; set; }
    [Column("payment_status")]
    public bool PaymentStatus { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
}
