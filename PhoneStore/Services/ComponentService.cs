using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class ComponentService : EntityCrudService<Component, ComponentFilter>
    {
        public ComponentService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<Component> ApplyEntityFilter(IQueryable<Component> query, ComponentFilter filter)
        {
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
                        query = query.Where(c => c.Title.Contains(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(c => c.Title.StartsWith(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(c => c.Title.EndsWith(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(c => c.Title.Equals(titleValue, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.ComponentCategoryId?.Value) && Guid.TryParse(filter.ComponentCategoryId.Value.Trim(), out var categoryId))
            {
                query = query.Where(c => c.ComponentCategoryId == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(filter.DataType?.Value) && Enum.TryParse<EDataType>(filter.DataType.Value.Trim(), true, out var dataType))
            {
                query = query.Where(c => c.DataType == dataType);
            }

            if (!string.IsNullOrWhiteSpace(filter.CategoryType?.Value) && Enum.TryParse<ECategoryType>(filter.CategoryType.Value.Trim(), true, out var categoryType))
            {
                query = query.Where(c => c.CategoryType == categoryType);
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "Title", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title);
                }
                else if (string.Equals(sortBy, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    query = descending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id);
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

        public IEnumerable<Component> GetAllComponents()
        {
            return GetAll();
        }

        public Component CreateComponent(Component component)
        {
            return Create(component);
        }

        public Component? UpdateComponent(Component component)
        {
            return Update(component);
        }

        public bool DeleteComponent(Guid id)
        {
            return Delete(id);
        }
    }
}
