using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class SkuService : EntityCrudService<Sku, SkuFilter>
    {
        public SkuService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<Sku> ApplyEntityFilter(IQueryable<Sku> query, SkuFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim().ToLower();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.Id.ToString().ToLower().Contains(idValue));
                }
                else
                {
                    query = query.Where(s => s.Id.ToString().ToLower() == idValue);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.ProductId?.Value) && Guid.TryParse(filter.ProductId.Value.Trim(), out var productId))
            {
                query = query.Where(s => s.ProductId == productId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Price?.Value) && double.TryParse(filter.Price.Value.Trim(), out var price))
            {
                switch (filter.Price.MatchMode?.ToLowerInvariant())
                {
                    case "greaterthan":
                        query = query.Where(s => s.Price > price);
                        break;
                    case "lessthan":
                        query = query.Where(s => s.Price < price);
                        break;
                    default:
                        query = query.Where(s => s.Price == price);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Amount?.Value) && double.TryParse(filter.Amount.Value.Trim(), out var amount))
            {
                switch (filter.Amount.MatchMode?.ToLowerInvariant())
                {
                    case "greaterthan":
                        query = query.Where(s => s.Amount > amount);
                        break;
                    case "lessthan":
                        query = query.Where(s => s.Amount < amount);
                        break;
                    default:
                        query = query.Where(s => s.Amount == amount);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Discount?.Value) && double.TryParse(filter.Discount.Value.Trim(), out var discount))
            {
                switch (filter.Discount.MatchMode?.ToLowerInvariant())
                {
                    case "greaterthan":
                        query = query.Where(s => s.Discount > discount);
                        break;
                    case "lessthan":
                        query = query.Where(s => s.Discount < discount);
                        break;
                    default:
                        query = query.Where(s => s.Discount == discount);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "Price", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(s => s.Price) : query.OrderBy(s => s.Price);
                }
                else if (string.Equals(sortBy, "Amount", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(s => s.Amount) : query.OrderBy(s => s.Amount);
                }
                else if (string.Equals(sortBy, "Discount", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(s => s.Discount) : query.OrderBy(s => s.Discount);
                }
                else
                {
                    query = descending ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id);
                }
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

            return query;
        }

        public IEnumerable<Sku> GetAllSkus()
        {
            return GetAll();
        }

        public Sku CreateSku(Sku sku)
        {
            return Create(sku);
        }

        public Sku? UpdateSku(Sku sku)
        {
            return Update(sku);
        }

        public bool DeleteSku(Guid id)
        {
            return Delete(id);
        }
    }
}
