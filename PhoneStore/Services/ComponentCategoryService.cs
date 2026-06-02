using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models.Filters;

namespace PhoneStore.Services
{
    public class ComponentCategoryService
    {
        private readonly ApplicationContext _db;

        public ComponentCategoryService(ApplicationContext db)
        {
            _db = db;
        }

        public ComponentCategory? GetById(Guid id)
        {
            return _db.ComponentCategories.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<ComponentCategory> GetAllCategories()
        {
            return _db.ComponentCategories.ToList();
        }

        public List<ComponentCategory> GetDataByFilter(ComponentCategoryFilter filter)
        {
            if (filter == null)
            {
                filter = new ComponentCategoryFilter();
            }

            var query = _db.ComponentCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Id.ToString().Contains(idValue, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    query = query.Where(c => c.Id.ToString().Equals(idValue, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Title?.Value))
            {
                var titleValue = filter.Title.Value.Trim();
                switch (filter.Title.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(c => c.Title != null && c.Title.Contains(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(c => c.Title != null && c.Title.StartsWith(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(c => c.Title != null && c.Title.EndsWith(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(c => c.Title != null && c.Title.Equals(titleValue, StringComparison.OrdinalIgnoreCase));
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
                        ? query.OrderByDescending(c => c.Title)
                        : query.OrderBy(c => c.Title);
                }
                else if (string.Equals(sortBy, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending
                        ? query.OrderByDescending(c => c.Id)
                        : query.OrderBy(c => c.Id);
                }
                else
                {
                    query = query.OrderBy(c => c.Title);
                }
            }
            else
            {
                query = query.OrderBy(c => c.Title);
            }

            return query
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();
        }

        public ComponentCategory CreateCategory(ComponentCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (category.Id == Guid.Empty)
            {
                category.Id = Guid.NewGuid();
            }

            _db.ComponentCategories.Add(category);
            _db.SaveChanges();
            return category;
        }

        public ComponentCategory? UpdateCategory(ComponentCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var existing = _db.ComponentCategories.FirstOrDefault(c => c.Id == category.Id);
            if (existing == null)
            {
                return null;
            }

            existing.Title = category.Title;
            _db.SaveChanges();
            return existing;
        }

        public bool DeleteCategory(Guid id)
        {
            var existing = _db.ComponentCategories.FirstOrDefault(c => c.Id == id);
            if (existing == null)
            {
                return false;
            }

            _db.ComponentCategories.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}
