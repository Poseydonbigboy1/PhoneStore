using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class ProductComponentService : EntityCrudService<ProductComponent, ProductComponentFilter>
    {
        public ProductComponentService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<ProductComponent> ApplyEntityFilter(IQueryable<ProductComponent> query, ProductComponentFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(pc => pc.Id.ToString().Contains(idValue, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    query = query.Where(pc => pc.Id.ToString().Equals(idValue, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.SkuId?.Value) && Guid.TryParse(filter.SkuId.Value.Trim(), out var skuId))
            {
                query = query.Where(pc => pc.SkuId == skuId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ComponentId?.Value) && Guid.TryParse(filter.ComponentId.Value.Trim(), out var componentId))
            {
                query = query.Where(pc => pc.ComponentId == componentId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ValueJson?.Value))
            {
                var valueJson = filter.ValueJson.Value.Trim();
                switch (filter.ValueJson.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(pc => pc.ValueJson.Contains(valueJson, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(pc => pc.ValueJson.StartsWith(valueJson, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(pc => pc.ValueJson.EndsWith(valueJson, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(pc => pc.ValueJson.Equals(valueJson, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Filtering?.Value) && bool.TryParse(filter.Filtering.Value.Trim(), out var filtering))
            {
                query = query.Where(pc => pc.Filtering == filtering);
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(pc => pc.Id) : query.OrderBy(pc => pc.Id);
                }
                else
                {
                    query = descending ? query.OrderByDescending(pc => pc.Id) : query.OrderBy(pc => pc.Id);
                }
            }
            else
            {
                query = query.OrderBy(pc => pc.Id);
            }

            return query;
        }

        public IEnumerable<ProductComponent> GetAllProductComponents()
        {
            return GetAll();
        }

        public ProductComponent CreateProductComponent(ProductComponent component)
        {
            return Create(component);
        }

        public ProductComponent? UpdateProductComponent(ProductComponent component)
        {
            return Update(component);
        }

        public bool DeleteProductComponent(Guid id)
        {
            return Delete(id);
        }
    }
}
