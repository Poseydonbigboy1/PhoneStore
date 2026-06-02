using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class OrderItemService : EntityCrudService<OrderItem, OrderItemFilter>
    {
        public OrderItemService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<OrderItem> ApplyEntityFilter(IQueryable<OrderItem> query, OrderItemFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(oi => oi.Id.ToString().Contains(idValue, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    query = query.Where(oi => oi.Id.ToString().Equals(idValue, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.OrderId?.Value) && Guid.TryParse(filter.OrderId.Value.Trim(), out var orderId))
            {
                query = query.Where(oi => oi.OrderId == orderId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SkuId?.Value) && Guid.TryParse(filter.SkuId.Value.Trim(), out var skuId))
            {
                query = query.Where(oi => oi.SkuId == skuId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Quantity?.Value) && int.TryParse(filter.Quantity.Value.Trim(), out var quantity))
            {
                switch (filter.Quantity.MatchMode?.ToLowerInvariant())
                {
                    case "greaterthan":
                        query = query.Where(oi => oi.Quantity > quantity);
                        break;
                    case "lessthan":
                        query = query.Where(oi => oi.Quantity < quantity);
                        break;
                    default:
                        query = query.Where(oi => oi.Quantity == quantity);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Price?.Value) && decimal.TryParse(filter.Price.Value.Trim(), out var price))
            {
                switch (filter.Price.MatchMode?.ToLowerInvariant())
                {
                    case "greaterthan":
                        query = query.Where(oi => oi.Price > price);
                        break;
                    case "lessthan":
                        query = query.Where(oi => oi.Price < price);
                        break;
                    default:
                        query = query.Where(oi => oi.Price == price);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "Price", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(oi => oi.Price) : query.OrderBy(oi => oi.Price);
                }
                else if (string.Equals(sortBy, "Quantity", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(oi => oi.Quantity) : query.OrderBy(oi => oi.Quantity);
                }
                else
                {
                    query = descending ? query.OrderByDescending(oi => oi.Id) : query.OrderBy(oi => oi.Id);
                }
            }
            else
            {
                query = query.OrderBy(oi => oi.Id);
            }

            return query;
        }

        public IEnumerable<OrderItem> GetAllOrderItems()
        {
            return GetAll();
        }

        public OrderItem CreateOrderItem(OrderItem orderItem)
        {
            return Create(orderItem);
        }

        public OrderItem? UpdateOrderItem(OrderItem orderItem)
        {
            return Update(orderItem);
        }

        public bool DeleteOrderItem(Guid id)
        {
            return Delete(id);
        }
    }
}
