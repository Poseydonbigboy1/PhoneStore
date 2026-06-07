namespace PhoneStore.Models.ViewModels;

public class DashboardViewModel
{
    public Dictionary<string, int> OrdersByStatus { get; set; } = new();
    public decimal RevenueToday { get; set; }
    public decimal Revenue7Days { get; set; }
    public decimal Revenue30Days { get; set; }
    public List<TopProductViewModel> TopProducts { get; set; } = new();
    public List<ZeroStockSkuViewModel> ZeroStockSkus { get; set; } = new();
}

public class TopProductViewModel
{
    public Guid SkuId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string BrandTitle { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class ZeroStockSkuViewModel
{
    public Guid SkuId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string BrandTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
