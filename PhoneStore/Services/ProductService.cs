using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class ProductService : EntityCrudService<Product, ProductFilter>
    {
        public ProductService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<Product> ApplyEntityFilter(IQueryable<Product> query, ProductFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim().ToLower();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(p => p.Id.ToString().ToLower().Contains(idValue));
                else
                    query = query.Where(p => p.Id.ToString().ToLower() == idValue);
            }

            if (!string.IsNullOrWhiteSpace(filter.Title?.Value))
            {
                var titleValue = filter.Title.Value.Trim().ToLower();
                switch (filter.Title.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(p => p.Title.ToLower().Contains(titleValue));
                        break;
                    case "startswith":
                        query = query.Where(p => p.Title.ToLower().StartsWith(titleValue));
                        break;
                    case "endswith":
                        query = query.Where(p => p.Title.ToLower().EndsWith(titleValue));
                        break;
                    default:
                        query = query.Where(p => p.Title.ToLower() == titleValue);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.BrandId?.Value) && Guid.TryParse(filter.BrandId.Value.Trim(), out var brandId))
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "Title", StringComparison.OrdinalIgnoreCase))
                    query = descending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
                else if (string.Equals(sortBy, "Id", StringComparison.OrdinalIgnoreCase))
                    query = descending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id);
                else
                    query = query.OrderBy(p => p.Title);
            }
            else
            {
                query = query.OrderBy(p => p.Title);
            }

            return query;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return GetAll();
        }

        public Product CreateProduct(Product product)
        {
            return Create(product);
        }

        public Product? UpdateProduct(Product product)
        {
            return Update(product);
        }

        public bool DeleteProduct(Guid id)
        {
            return Delete(id);
        }
    }
}
