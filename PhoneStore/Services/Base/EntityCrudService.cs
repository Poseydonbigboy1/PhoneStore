using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Services.Base
{
    public abstract class EntityCrudService<TEntity, TFilter>
        where TEntity : class, IEntity
        where TFilter : FilterBase, new()
    {
        protected readonly ApplicationContext _db;

        protected EntityCrudService(ApplicationContext db)
        {
            _db = db;
        }

        protected virtual DbSet<TEntity> Entities => _db.Set<TEntity>();

        protected abstract IQueryable<TEntity> ApplyEntityFilter(IQueryable<TEntity> query, TFilter filter);

        public TEntity? GetById(Guid id)
        {
            return Entities.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Entities.ToList();
        }

        public FilterResult<TEntity> GetDataByFilter(TFilter filter)
        {
            if (filter == null)
            {
                filter = new TFilter();
            }

            var query = ApplyEntityFilter(Entities, filter);
            var total = query.Count();
            var items = query
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();

            return new FilterResult<TEntity> { Items = items, Total = total };
        }

        public TEntity Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            Entities.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public TEntity? Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var existing = Entities.Find(entity.Id);
            if (existing == null)
            {
                return null;
            }

            _db.Entry(existing).CurrentValues.SetValues(entity);
            _db.SaveChanges();
            return existing;
        }

        public bool Delete(Guid id)
        {
            var existing = Entities.Find(id);
            if (existing == null)
            {
                return false;
            }

            Entities.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}
