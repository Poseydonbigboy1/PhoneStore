using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Data;
using PhoneStore.Services;

namespace PhoneStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<Order>>> GetAll()
        {
            try
            {
                var data = _orderService.GetAllOrders();
                return ResultObject<IEnumerable<Order>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<Order>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<FilterResult<Order>>> GetByFilter([FromBody] OrderFilter filter)
        {
            try
            {
                var data = _orderService.GetDataByFilter(filter);
                return ResultObject<FilterResult<Order>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<FilterResult<Order>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Order>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _orderService.GetById(id);
                if (item == null) return ResultObject<Order>.Error("Order not found");
                return ResultObject<Order>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<Order>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<Order>> Create([FromBody] Order order)
        {
            try
            {
                var created = _orderService.CreateOrder(order);
                return ResultObject<Order>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<Order>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<Order>> Update([FromBody] Order order)
        {
            try
            {
                var updated = _orderService.UpdateOrder(order);
                if (updated == null) return ResultObject<Order>.Error("Order not found");
                return ResultObject<Order>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<Order>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _orderService.DeleteOrder(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
