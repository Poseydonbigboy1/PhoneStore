using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Services.Base
{
    public abstract class EntityCrudService<TEntity, TFilter>
        where TEntity : class
        where TFilter : FilterBase, new()
    {
        protected readonly ApplicationContext _db;

        protected EntityCrudService(ApplicationContext db)
        {
            _db = db;
        }

        protected abstract IQueryable<TEntity> DbSet { get; }
        protected abstract TEntity? FindById(Guid id);
        protected abstract void AttachNewEntity(TEntity entity);
        protected abstract void RemoveEntity(TEntity entity);
        protected abstract void CopyUpdatedValues(TEntity existing, TEntity updated);
        protected abstract bool IsNew(TEntity entity);
        protected abstract void InitializeEntityId(TEntity entity);
        protected abstract Guid GetEntityId(TEntity entity);
        protected abstract IQueryable<TEntity> ApplyEntityFilter(IQueryable<TEntity> query, TFilter filter);

        public TEntity? GetById(Guid id)
        {
            return FindById(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public List<TEntity> GetDataByFilter(TFilter filter)
        {
            if (filter == null)
            {
                filter = new TFilter();
            }

            return ApplyEntityFilter(DbSet, filter)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();
        }

        public TEntity Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (IsNew(entity))
            {
                InitializeEntityId(entity);
            }

            AttachNewEntity(entity);
            _db.SaveChanges();
            return entity;
        }

        public TEntity? Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var existing = FindById(GetEntityId(entity));
            if (existing == null)
            {
                return null;
            }

            CopyUpdatedValues(existing, entity);
            _db.SaveChanges();
            return existing;
        }

        public bool Delete(Guid id)
        {
            var existing = FindById(id);
            if (existing == null)
            {
                return false;
            }

            RemoveEntity(existing);
            _db.SaveChanges();
            return true;
        }
    }
}
