using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models.ViewModels;
using System.Linq;

namespace PhoneStore.Services;

public class AnalyticsService
{
    private readonly ApplicationContext _db;

    public AnalyticsService(ApplicationContext db) => _db = db;

    /// <summary>
    /// Npgsql требует DateTimeKind.Utc для timestamp with time zone.
    /// .Date и new DateTime() снимают Kind — используем этот метод везде.
    /// </summary>
    private static DateTime Utc(DateTime d) =>
        d.Kind == DateTimeKind.Utc ? d : DateTime.SpecifyKind(d, DateTimeKind.Utc);

    /// <summary>Выручка и заказы по периодам (groupBy: day | week | month)</summary>
    public async Task<List<RevenuePointViewModel>> GetRevenueAsync(
        DateTime from, DateTime to, string groupBy = "day")
    {
        var f = Utc(from);
        var t = Utc(to);

        var orders = await _db.Orders
            .Where(o => o.Status != EOrderStatus.Cancelled
                     && o.OrderDate >= f && o.OrderDate <= t)
            .Select(o => new { o.OrderDate, o.TotalAmount })
            .ToListAsync();

        // In-memory grouping: ключи — только для словаря, не идут в Postgres
        var grouped = groupBy switch
        {
            "week"  => orders.GroupBy(o => CultureWeekKey(o.OrderDate)).ToDictionary(g => g.Key, g => g.ToList()),
            "month" => orders.GroupBy(o => new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1, 0, 0, 0, DateTimeKind.Utc)).ToDictionary(g => g.Key, g => g.ToList()),
            _       => orders.GroupBy(o => o.OrderDate.Date).ToDictionary(g => g.Key, g => g.ToList()),
        };

        var result = new List<RevenuePointViewModel>();
        for (var d = f.Date; d <= t.Date; )
        {
            var key = groupBy switch
            {
                "week"  => CultureWeekKey(d),
                "month" => new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                _       => d,
            };

            if (grouped.TryGetValue(key, out var items))
            {
                result.Add(new RevenuePointViewModel
                {
                    Date        = key,
                    Revenue     = items.Sum(x => x.TotalAmount),
                    OrdersCount = items.Count,
                });
            }
            else
            {
                result.Add(new RevenuePointViewModel { Date = key, Revenue = 0, OrdersCount = 0 });
            }

            d = groupBy switch
            {
                "week"  => d.AddDays(7),
                "month" => d.AddMonths(1),
                _       => d.AddDays(1),
            };
        }

        return result.DistinctBy(r => r.Date).OrderBy(r => r.Date).ToList();
    }

    /// <summary>Топ SKU по выручке</summary>
    public async Task<List<TopProductAnalyticsViewModel>> GetTopProductsAsync(
        DateTime from, DateTime to, int take = 10)
    {
        var f = Utc(from);
        var t = Utc(to);

        var total = await _db.OrderItems
            .Where(oi => oi.Order!.Status != EOrderStatus.Cancelled
                      && oi.Order.OrderDate >= f && oi.Order.OrderDate <= t)
            .SumAsync(oi => (decimal?)oi.Price * oi.Quantity) ?? 0;

        // Group in memory to avoid EF Core translation issues with g.First().Sku
        var rawItems = await _db.OrderItems
            .Include(oi => oi.Sku).ThenInclude(s => s!.Product).ThenInclude(p => p!.Brand)
            .Where(oi => oi.Order!.Status != EOrderStatus.Cancelled
                      && oi.Order!.OrderDate >= f && oi.Order!.OrderDate <= t)
            .ToListAsync();

        var items = rawItems
            .GroupBy(oi => oi.SkuId)
            .Select(g => new
            {
                SkuId        = g.Key,
                TotalSold    = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity),
                Sku          = g.First().Sku,
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(take)
            .ToList();

        return items.Select(x => new TopProductAnalyticsViewModel
        {
            SkuId        = x.SkuId,
            ProductTitle = x.Sku?.Product?.Title ?? string.Empty,
            BrandTitle   = x.Sku?.Product?.Brand?.Title ?? string.Empty,
            TotalSold    = x.TotalSold,
            TotalRevenue = x.TotalRevenue,
            SharePercent = total > 0 ? Math.Round((double)(x.TotalRevenue / total) * 100, 1) : 0,
        }).ToList();
    }

    /// <summary>Новые клиенты — считаем по первому заказу в период</summary>
    public async Task<List<NewCustomersPointViewModel>> GetNewCustomersAsync(
        DateTime from, DateTime to, string groupBy = "day")
    {
        var f = Utc(from);
        var t = Utc(to);

        // Считаем новыми тех, чей первый заказ попал в диапазон
        var firstOrders = await _db.Orders
            .Where(o => o.OrderDate >= f && o.OrderDate <= t)
            .GroupBy(o => o.UserId)
            .Select(g => g.Min(o => o.OrderDate))
            .ToListAsync();

        var grouped = groupBy switch
        {
            "week"  => firstOrders.GroupBy(d => CultureWeekKey(d)).ToDictionary(g => g.Key, g => g.Count()),
            "month" => firstOrders.GroupBy(d => new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc)).ToDictionary(g => g.Key, g => g.Count()),
            _       => firstOrders.GroupBy(d => d.Date).ToDictionary(g => g.Key, g => g.Count()),
        };

        var result = new List<NewCustomersPointViewModel>();
        for (var d = f.Date; d <= t.Date; )
        {
            var key = groupBy switch
            {
                "week"  => CultureWeekKey(d),
                "month" => new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                _       => d,
            };
            result.Add(new NewCustomersPointViewModel
            {
                Date  = key,
                Count = grouped.GetValueOrDefault(key, 0),
            });
            d = groupBy switch
            {
                "week"  => d.AddDays(7),
                "month" => d.AddMonths(1),
                _       => d.AddDays(1),
            };
        }

        return result.DistinctBy(r => r.Date).OrderBy(r => r.Date).ToList();
    }

    /// <summary>Распределение заказов по статусам</summary>
    public async Task<Dictionary<string, int>> GetOrdersByStatusAsync(DateTime from, DateTime to)
    {
        var f = Utc(from);
        var t = Utc(to);
        return await _db.Orders
            .Where(o => o.OrderDate >= f && o.OrderDate <= t)
            .GroupBy(o => o.Status)
            .ToDictionaryAsync(g => g.Key.ToString(), g => g.Count());
    }

    /// <summary>Сводные метрики среднего чека</summary>
    public async Task<AvgOrderViewModel> GetAvgOrderAsync(DateTime from, DateTime to)
    {
        var f = Utc(from);
        var t = Utc(to);
        var amounts = await _db.Orders
            .Where(o => o.Status != EOrderStatus.Cancelled && o.OrderDate >= f && o.OrderDate <= t)
            .Select(o => (decimal)o.TotalAmount)
            .ToListAsync();

        if (!amounts.Any())
            return new AvgOrderViewModel();

        amounts.Sort();
        return new AvgOrderViewModel
        {
            AvgAmount    = Math.Round(amounts.Average(), 0),
            MedianAmount = amounts.Count % 2 == 0
                ? Math.Round((amounts[amounts.Count / 2 - 1] + amounts[amounts.Count / 2]) / 2, 0)
                : amounts[amounts.Count / 2],
            MaxAmount = amounts.Max(),
            TotalOrders = amounts.Count,
        };
    }

    private static DateTime CultureWeekKey(DateTime d)
    {
        // Start of ISO week (Monday), Kind сохраняем как Utc
        int diff = (7 + (d.DayOfWeek - DayOfWeek.Monday)) % 7;
        return DateTime.SpecifyKind(d.Date.AddDays(-diff), DateTimeKind.Utc);
    }
}
