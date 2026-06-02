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

        protected override IQueryable<ComponentCategory> DbSet => _db.ComponentCategories;

        protected override ComponentCategory? FindById(Guid id)
        {
            return _db.ComponentCategories.FirstOrDefault(c => c.Id == id);
        }

        protected override void AttachNewEntity(ComponentCategory entity)
        {
            _db.ComponentCategories.Add(entity);
        }

        protected override void RemoveEntity(ComponentCategory entity)
        {
            _db.ComponentCategories.Remove(entity);
        }

        protected override void CopyUpdatedValues(ComponentCategory existing, ComponentCategory updated)
        {
            existing.Title = updated.Title;
        }

        protected override bool IsNew(ComponentCategory entity)
        {
            return entity.Id == Guid.Empty;
        }

        protected override void InitializeEntityId(ComponentCategory entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Guid GetEntityId(ComponentCategory entity)
        {
            return entity.Id;
        }

        protected override IQueryable<ComponentCategory> ApplyEntityFilter(IQueryable<ComponentCategory> query, ComponentCategoryFilter filter)
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
