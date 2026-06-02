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

        protected override IQueryable<Brand> DbSet => _db.Brands;

        protected override Brand? FindById(Guid id)
        {
            return _db.Brands.FirstOrDefault(b => b.Id == id);
        }

        protected override void AttachNewEntity(Brand entity)
        {
            _db.Brands.Add(entity);
        }

        protected override void RemoveEntity(Brand entity)
        {
            _db.Brands.Remove(entity);
        }

        protected override void CopyUpdatedValues(Brand existing, Brand updated)
        {
            existing.Title = updated.Title;
        }

        protected override bool IsNew(Brand entity)
        {
            return entity.Id == Guid.Empty;
        }

        protected override void InitializeEntityId(Brand entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Guid GetEntityId(Brand entity)
        {
            return entity.Id;
        }

        protected override IQueryable<Brand> ApplyEntityFilter(IQueryable<Brand> query, BrandFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => b.Id.ToString().Contains(idValue, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    query = query.Where(b => b.Id.ToString().Equals(idValue, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Title?.Value))
            {
                var titleValue = filter.Title.Value.Trim();
                switch (filter.Title.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(b => b.Title != null && b.Title.Contains(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(b => b.Title != null && b.Title.StartsWith(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(b => b.Title != null && b.Title.EndsWith(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(b => b.Title != null && b.Title.Equals(titleValue, StringComparison.OrdinalIgnoreCase));
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
