using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class BrandService : EntityCrudService<Brand, BrandFilter>
    {
        public BrandService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<Brand> ApplyEntityFilter(IQueryable<Brand> query, BrandFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim().ToLower();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => b.Id.ToString().ToLower().Contains(idValue));
                }
                else
                {
                    query = query.Where(b => b.Id.ToString().ToLower() == idValue);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Title?.Value))
            {
                var titleValue = filter.Title.Value.Trim().ToLower();
                switch (filter.Title.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(b => b.Title != null && b.Title.ToLower().Contains(titleValue));
                        break;
                    case "startswith":
                        query = query.Where(b => b.Title != null && b.Title.ToLower().StartsWith(titleValue));
                        break;
                    case "endswith":
                        query = query.Where(b => b.Title != null && b.Title.ToLower().EndsWith(titleValue));
                        break;
                    default:
                        query = query.Where(b => b.Title != null && b.Title.ToLower() == titleValue);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;

                if (string.Equals(sortBy, "Title", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending
                        ? query.OrderByDescending(b => b.Title)
                        : query.OrderBy(b => b.Title);
                }
                else if (string.Equals(sortBy, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending
                        ? query.OrderByDescending(b => b.Id)
                        : query.OrderBy(b => b.Id);
                }
                else
                {
                    query = query.OrderBy(b => b.Title);
                }
            }
            else
            {
                query = query.OrderBy(b => b.Title);
            }

            return query;
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return GetAll();
        }

        public Brand CreateBrand(Brand brand)
        {
            return Create(brand);
        }

        public Brand? UpdateBrand(Brand brand)
        {
            return Update(brand);
        }

        public bool DeleteBrand(Guid id)
        {
            return Delete(id);
        }
    }
}
