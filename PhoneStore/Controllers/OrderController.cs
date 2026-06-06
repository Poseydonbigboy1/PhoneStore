using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Models.RequestModels;
using PhoneStore.Models.ViewModels;
using PhoneStore.Services;
using System.Security.Claims;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService        _orderService;
    private readonly CheckoutService     _checkoutService;
    private readonly OrderManagerService _managerService;

    public OrderController(OrderService orderService, CheckoutService checkoutService, OrderManagerService managerService)
    {
        _orderService    = orderService;
        _checkoutService = checkoutService;
        _managerService  = managerService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // ─── MANAGER CRUD ────────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = "MANAGER")]
    public ActionResult<ResultObject<IEnumerable<Order>>> GetAll()
    {
        try { return ResultObject<IEnumerable<Order>>.Success(_orderService.GetAllOrders()); }
        catch (Exception ex) { return ResultObject<IEnumerable<Order>>.Error(ex); }
    }

    [HttpPost("filter")]
    [Authorize(Roles = "MANAGER")]
    public ActionResult<ResultObject<FilterResult<Order>>> GetByFilter([FromBody] OrderFilter filter)
    {
        try { return ResultObject<FilterResult<Order>>.Success(_orderService.GetDataByFilter(filter)); }
        catch (Exception ex) { return ResultObject<FilterResult<Order>>.Error(ex); }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "MANAGER")]
    public ActionResult<ResultObject<Order>> GetById([FromRoute] Guid id)
    {
        try
        {
            var item = _orderService.GetById(id);
            return item is null ? ResultObject<Order>.Error("Order not found") : ResultObject<Order>.Success(item);
        }
        catch (Exception ex) { return ResultObject<Order>.Error(ex); }
    }

    [HttpPost]
    [Authorize(Roles = "MANAGER")]
    public ActionResult<ResultObject<Order>> Create([FromBody] Order order)
    {
        try { return ResultObject<Order>.Success(_orderService.CreateOrder(order)); }
        catch (Exception ex) { return ResultObject<Order>.Error(ex); }
    }

    [HttpPut]
    [Authorize(Roles = "MANAGER")]
    public ActionResult<ResultObject<Order>> Update([FromBody] Order order)
    {
        try
        {
            var updated = _orderService.UpdateOrder(order);
            return updated is null ? ResultObject<Order>.Error("Order not found") : ResultObject<Order>.Success(updated);
        }
        catch (Exception ex) { return ResultObject<Order>.Error(ex); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "MANAGER")]
    public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
    {
        try { return ResultObject<bool>.Success(_orderService.DeleteOrder(id)); }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }

    // ─── MANAGER: смена статуса ───────────────────────────────────────

    [HttpPut("{id}/status")]
    [Authorize(Roles = "MANAGER")]
    public async Task<ActionResult<ResultObject<bool>>> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeStatusRequest req)
    {
        try
        {
            await _managerService.ChangeStatusAsync(id, req.Status);
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }

    // ─── CUSTOMER: checkout ───────────────────────────────────────────

    [HttpPost("checkout")]
    [Authorize]
    public async Task<ActionResult<ResultObject<object>>> Checkout([FromBody] CheckoutRequest req)
    {
        try
        {
            var orderId = await _checkoutService.CheckoutAsync(GetUserId(), req);
            return ResultObject<object>.Success(new { orderId });
        }
        catch (Exception ex) { return ResultObject<object>.Error(ex); }
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<ResultObject<List<OrderSummaryViewModel>>>> GetMyOrders()
    {
        try
        {
            var orders = await _checkoutService.GetMyOrdersAsync(GetUserId());
            return ResultObject<List<OrderSummaryViewModel>>.Success(orders);
        }
        catch (Exception ex) { return ResultObject<List<OrderSummaryViewModel>>.Error(ex); }
    }

    [HttpGet("my/{id}")]
    [Authorize]
    public async Task<ActionResult<ResultObject<OrderDetailViewModel>>> GetMyOrderDetail([FromRoute] Guid id)
    {
        try
        {
            var order = await _checkoutService.GetMyOrderDetailAsync(GetUserId(), id);
            return order is null ? ResultObject<OrderDetailViewModel>.Error("Заказ не найден") : ResultObject<OrderDetailViewModel>.Success(order);
        }
        catch (Exception ex) { return ResultObject<OrderDetailViewModel>.Error(ex); }
    }
}

public class ChangeStatusRequest
{
    public EOrderStatus Status { get; set; }
}
