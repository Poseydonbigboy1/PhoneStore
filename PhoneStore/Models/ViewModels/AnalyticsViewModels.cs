namespace PhoneStore.Models.ViewModels;

public class RevenuePointViewModel
{
    public DateTime Date        { get; set; }
    public decimal  Revenue     { get; set; }
    public int      OrdersCount { get; set; }
}

public class TopProductAnalyticsViewModel
{
    public Guid    SkuId        { get; set; }
    public string  ProductTitle { get; set; } = string.Empty;
    public string  BrandTitle   { get; set; } = string.Empty;
    public int     TotalSold    { get; set; }
    public decimal TotalRevenue { get; set; }
    public double  SharePercent { get; set; }
}

public class NewCustomersPointViewModel
{
    public DateTime Date  { get; set; }
    public int      Count { get; set; }
}

public class AvgOrderViewModel
{
    public decimal AvgAmount    { get; set; }
    public decimal MedianAmount { get; set; }
    public decimal MaxAmount    { get; set; }
    public int     TotalOrders  { get; set; }
}
