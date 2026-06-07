using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models.ViewModels;
using PhoneStore.Services;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "MANAGER")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analytics;

    public AnalyticsController(AnalyticsService analytics) => _analytics = analytics;

    /// <summary>Конвертирует DateTime в UTC (Kind=Utc). .Date снимает Kind — нужно явно указывать.</summary>
    private static DateTime Utc(DateTime d) => DateTime.SpecifyKind(d, DateTimeKind.Utc);

    private static DateTime FromBound(DateTime? d) =>
        Utc((d?.Kind == DateTimeKind.Utc ? d.Value : (d ?? DateTime.UtcNow.AddDays(-30))).Date);

    private static DateTime ToBound(DateTime? d) =>
        Utc((d?.Kind == DateTimeKind.Utc ? d.Value : (d ?? DateTime.UtcNow)).Date).AddDays(1).AddSeconds(-1);

    /// <summary>Выручка и кол-во заказов по периодам</summary>
    [HttpGet("revenue")]
    public async Task<ActionResult<ResultObject<List<RevenuePointViewModel>>>> GetRevenue(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            var data = await _analytics.GetRevenueAsync(FromBound(from), ToBound(to), groupBy);
            return ResultObject<List<RevenuePointViewModel>>.Success(data);
        }
        catch (Exception ex) { return ResultObject<List<RevenuePointViewModel>>.Error(ex); }
    }

    /// <summary>Топ товаров по выручке</summary>
    [HttpGet("top-products")]
    public async Task<ActionResult<ResultObject<List<TopProductAnalyticsViewModel>>>> GetTopProducts(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int take = 10)
    {
        try
        {
            var data = await _analytics.GetTopProductsAsync(FromBound(from), ToBound(to), take);
            return ResultObject<List<TopProductAnalyticsViewModel>>.Success(data);
        }
        catch (Exception ex) { return ResultObject<List<TopProductAnalyticsViewModel>>.Error(ex); }
    }

    /// <summary>Новые клиенты (первый заказ) по периодам</summary>
    [HttpGet("new-customers")]
    public async Task<ActionResult<ResultObject<List<NewCustomersPointViewModel>>>> GetNewCustomers(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            var data = await _analytics.GetNewCustomersAsync(FromBound(from), ToBound(to), groupBy);
            return ResultObject<List<NewCustomersPointViewModel>>.Success(data);
        }
        catch (Exception ex) { return ResultObject<List<NewCustomersPointViewModel>>.Error(ex); }
    }

    /// <summary>Заказы по статусам за период</summary>
    [HttpGet("orders-by-status")]
    public async Task<ActionResult<ResultObject<Dictionary<string, int>>>> GetOrdersByStatus(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        try
        {
            var data = await _analytics.GetOrdersByStatusAsync(FromBound(from), ToBound(to));
            return ResultObject<Dictionary<string, int>>.Success(data);
        }
        catch (Exception ex) { return ResultObject<Dictionary<string, int>>.Error(ex); }
    }

    /// <summary>Средний чек за период</summary>
    [HttpGet("avg-order")]
    public async Task<ActionResult<ResultObject<AvgOrderViewModel>>> GetAvgOrder(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        try
        {
            var data = await _analytics.GetAvgOrderAsync(FromBound(from), ToBound(to));
            return ResultObject<AvgOrderViewModel>.Success(data);
        }
        catch (Exception ex) { return ResultObject<AvgOrderViewModel>.Error(ex); }
    }
}
