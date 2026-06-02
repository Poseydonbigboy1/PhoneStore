using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;

namespace PhoneStore.Services
{
    public class BrandService
    {
        private readonly ApplicationContext _db;

        public BrandService(ApplicationContext db)
        {
            _db = db;
        }

        public Brand? GetById(Guid id)
        {
            return _db.Brands.FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return _db.Brands.ToList();
        }

        public List<Brand> GetDataByFilter(BrandFilter filter)
        {
            if (filter == null)
            {
                filter = new BrandFilter();
            }

            var query = _db.Brands.AsQueryable();

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

            return query
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();
        }

        public Brand CreateBrand(Brand brand)
        {
            if (brand == null)
            {
                throw new ArgumentNullException(nameof(brand));
            }

            if (brand.Id == Guid.Empty)
            {
                brand.Id = Guid.NewGuid();
            }

            _db.Brands.Add(brand);
            _db.SaveChanges();
            return brand;
        }

        public Brand? UpdateBrand(Brand brand)
        {
            if (brand == null)
            {
                throw new ArgumentNullException(nameof(brand));
            }

            var existing = _db.Brands.FirstOrDefault(b => b.Id == brand.Id);
            if (existing == null)
            {
                return null;
            }

            existing.Title = brand.Title;
            _db.SaveChanges();
            return existing;
        }

        public bool DeleteBrand(Guid id)
        {
            var existing = _db.Brands.FirstOrDefault(b => b.Id == id);
            if (existing == null)
            {
                return false;
            }

            _db.Brands.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}