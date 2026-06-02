using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class OrderService : EntityCrudService<Order, OrderFilter>
    {
        public OrderService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<Order> ApplyEntityFilter(IQueryable<Order> query, OrderFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(o => o.Id.ToString().Contains(idValue, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    query = query.Where(o => o.Id.ToString().Equals(idValue, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.UserId?.Value) && Guid.TryParse(filter.UserId.Value.Trim(), out var userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status?.Value) && Enum.TryParse<EOrderStatus>(filter.Status.Value.Trim(), true, out var status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(filter.ShippingAddress?.Value))
            {
                var addressValue = filter.ShippingAddress.Value.Trim();
                switch (filter.ShippingAddress.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(o => o.ShippingAddress.Contains(addressValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(o => o.ShippingAddress.StartsWith(addressValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(o => o.ShippingAddress.EndsWith(addressValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(o => o.ShippingAddress.Equals(addressValue, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.TotalAmount?.Value) && decimal.TryParse(filter.TotalAmount.Value.Trim(), out var totalAmount))
            {
                switch (filter.TotalAmount.MatchMode?.ToLowerInvariant())
                {
                    case "greaterthan":
                        query = query.Where(o => o.TotalAmount > totalAmount);
                        break;
                    case "lessthan":
                        query = query.Where(o => o.TotalAmount < totalAmount);
                        break;
                    default:
                        query = query.Where(o => o.TotalAmount == totalAmount);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "OrderDate", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate);
                }
                else if (string.Equals(sortBy, "TotalAmount", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount);
                }
                else
                {
                    query = descending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate);
                }
            }
            else
            {
                query = query.OrderBy(o => o.OrderDate);
            }

            return query;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return GetAll();
        }

        public Order CreateOrder(Order order)
        {
            return Create(order);
        }

        public Order? UpdateOrder(Order order)
        {
            return Update(order);
        }

        public bool DeleteOrder(Guid id)
        {
            return Delete(id);
        }
    }
}
