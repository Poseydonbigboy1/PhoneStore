using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models.ViewModels;

namespace PhoneStore.Services;

public class DashboardService
{
    private readonly ApplicationContext _db;

    public DashboardService(ApplicationContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var now   = DateTime.UtcNow;
        // .Date снимает Kind — явно указываем Utc, иначе Npgsql бросит исключение
        var today = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
        var week  = DateTime.SpecifyKind(now.Date.AddDays(-7),  DateTimeKind.Utc);
        var month = DateTime.SpecifyKind(now.Date.AddDays(-30), DateTimeKind.Utc);

        // Заказы по статусам
        var ordersByStatus = await _db.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        // Выручка (только не-отменённые)
        var revenueToday = await _db.Orders
            .Where(o => o.Status != EOrderStatus.Cancelled && o.OrderDate >= today)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        var revenue7 = await _db.Orders
            .Where(o => o.Status != EOrderStatus.Cancelled && o.OrderDate >= week)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        var revenue30 = await _db.Orders
            .Where(o => o.Status != EOrderStatus.Cancelled && o.OrderDate >= month)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        // Топ-5 по кол-ву продаж
        var topProducts = await _db.OrderItems
            .Include(oi => oi.Sku).ThenInclude(s => s!.Product).ThenInclude(p => p!.Brand)
            .GroupBy(oi => oi.SkuId)
            .Select(g => new
            {
                SkuId      = g.Key,
                TotalSold  = g.Sum(oi => oi.Quantity),
                Revenue    = g.Sum(oi => oi.Price * oi.Quantity),
                Sku        = g.First().Sku,
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToListAsync();

        // SKU с нулевым остатком
        var zeroStock = await _db.Skus
            .Include(s => s.Product).ThenInclude(p => p!.Brand)
            .Where(s => s.Amount == 0)
            .OrderBy(s => s.Product!.Title)
            .Take(20)
            .ToListAsync();

        return new DashboardViewModel
        {
            OrdersByStatus = ordersByStatus.ToDictionary(x => x.Status, x => x.Count),
            RevenueToday   = revenueToday,
            Revenue7Days   = revenue7,
            Revenue30Days  = revenue30,
            TopProducts = topProducts.Select(x => new TopProductViewModel
            {
                SkuId        = x.SkuId,
                ProductTitle = x.Sku?.Product?.Title ?? string.Empty,
                BrandTitle   = x.Sku?.Product?.Brand?.Title ?? string.Empty,
                TotalSold    = x.TotalSold,
                TotalRevenue = (decimal)x.Revenue,
            }).ToList(),
            ZeroStockSkus = zeroStock.Select(s => new ZeroStockSkuViewModel
            {
                SkuId        = s.Id,
                ProductTitle = s.Product?.Title ?? string.Empty,
                BrandTitle   = s.Product?.Brand?.Title ?? string.Empty,
                Price        = (decimal)s.Price,
            }).ToList(),
        };
    }
}
