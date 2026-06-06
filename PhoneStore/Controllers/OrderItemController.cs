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
    public class OrderItemController : ControllerBase
    {
        private readonly OrderItemService _orderItemService;

        public OrderItemController(OrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<OrderItem>>> GetAll()
        {
            try
            {
                var data = _orderItemService.GetAllOrderItems();
                return ResultObject<IEnumerable<OrderItem>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<OrderItem>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<FilterResult<OrderItem>>> GetByFilter([FromBody] OrderItemFilter filter)
        {
            try
            {
                var data = _orderItemService.GetDataByFilter(filter);
                return ResultObject<FilterResult<OrderItem>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<FilterResult<OrderItem>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<OrderItem>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _orderItemService.GetById(id);
                if (item == null) return ResultObject<OrderItem>.Error("OrderItem not found");
                return ResultObject<OrderItem>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<OrderItem>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<OrderItem>> Create([FromBody] OrderItem orderItem)
        {
            try
            {
                var created = _orderItemService.CreateOrderItem(orderItem);
                return ResultObject<OrderItem>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<OrderItem>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<OrderItem>> Update([FromBody] OrderItem orderItem)
        {
            try
            {
                var updated = _orderItemService.UpdateOrderItem(orderItem);
                if (updated == null) return ResultObject<OrderItem>.Error("OrderItem not found");
                return ResultObject<OrderItem>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<OrderItem>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _orderItemService.DeleteOrderItem(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
