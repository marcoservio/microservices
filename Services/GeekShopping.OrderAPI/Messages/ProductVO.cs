﻿namespace GeekShopping.OrderAPI.Messages;

public class ProductVO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
}
