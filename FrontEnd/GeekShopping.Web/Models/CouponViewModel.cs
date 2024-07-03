﻿namespace GeekShopping.Web.Models;

public class CouponViewModel
{
    public long Id { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
}
