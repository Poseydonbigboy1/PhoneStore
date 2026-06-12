using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class ComponentCategoryService : EntityCrudService<ComponentCategory, ComponentCategoryFilter>
    {
        public ComponentCategoryService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<ComponentCategory> ApplyEntityFilter(IQueryable<ComponentCategory> query, ComponentCategoryFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim().ToLower();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Id.ToString().ToLower().Contains(idValue));
                }
                else
                {
                    query = query.Where(c => c.Id.ToString().ToLower() == idValue);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Title?.Value))
            {
                var titleValue = filter.Title.Value.Trim().ToLower();
                switch (filter.Title.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(c => c.Title != null && c.Title.ToLower().Contains(titleValue));
                        break;
                    case "startswith":
                        query = query.Where(c => c.Title != null && c.Title.ToLower().StartsWith(titleValue));
                        break;
                    case "endswith":
                        query = query.Where(c => c.Title != null && c.Title.ToLower().EndsWith(titleValue));
                        break;
                    default:
                        query = query.Where(c => c.Title != null && c.Title.ToLower() == titleValue);
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

            return query;
        }

        public IEnumerable<ComponentCategory> GetAllCategories()
        {
            return GetAll();
        }

        public ComponentCategory CreateCategory(ComponentCategory category)
        {
            return Create(category);
        }

        public ComponentCategory? UpdateCategory(ComponentCategory category)
        {
            return Update(category);
        }

        public bool DeleteCategory(Guid id)
        {
            return Delete(id);
        }
    }
}
