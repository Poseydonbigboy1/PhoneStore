using System;
using System.Collections.Generic;
using System.Linq;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services.Base;

namespace PhoneStore.Services
{
    public class UserService : EntityCrudService<User, UserFilter>
    {
        public UserService(ApplicationContext db)
            : base(db)
        {
        }

        protected override IQueryable<User> ApplyEntityFilter(IQueryable<User> query, UserFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Id?.Value))
            {
                var idValue = filter.Id.Value.Trim();
                if (string.Equals(filter.Id.MatchMode, "contains", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(u => u.Id.ToString().Contains(idValue, StringComparison.OrdinalIgnoreCase));
                else
                    query = query.Where(u => u.Id.ToString().Equals(idValue, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Login?.Value))
            {
                var loginValue = filter.Login.Value.Trim();
                switch (filter.Login.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(u => u.Login.Contains(loginValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(u => u.Login.StartsWith(loginValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(u => u.Login.EndsWith(loginValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(u => u.Login.Equals(loginValue, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Name?.Value))
            {
                var nameValue = filter.Name.Value.Trim();
                switch (filter.Name.MatchMode?.ToLowerInvariant())
                {
                    case "contains":
                        query = query.Where(u => u.Name != null && u.Name.Contains(nameValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "startswith":
                        query = query.Where(u => u.Name != null && u.Name.StartsWith(nameValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "endswith":
                        query = query.Where(u => u.Name != null && u.Name.EndsWith(nameValue, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        query = query.Where(u => u.Name != null && u.Name.Equals(nameValue, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Roles?.Value) && Enum.TryParse<ERole>(filter.Roles.Value.Trim(), true, out var role))
            {
                query = query.Where(u => u.Roles == role);
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortBy = filter.SortBy.Trim();
                var descending = filter.SortDirection == SortDirection.Descending;
                if (string.Equals(sortBy, "Login", StringComparison.OrdinalIgnoreCase))
                    query = descending ? query.OrderByDescending(u => u.Login) : query.OrderBy(u => u.Login);
                else if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                    query = descending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name);
                else if (string.Equals(sortBy, "Id", StringComparison.OrdinalIgnoreCase))
                    query = descending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id);
                else
                    query = query.OrderBy(u => u.Login);
            }
            else
            {
                query = query.OrderBy(u => u.Login);
            }

            return query;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return GetAll();
        }

        public User CreateUser(User user)
        {
            return Create(user);
        }

        public User? UpdateUser(User user)
        {
            return Update(user);
        }

        public bool DeleteUser(Guid id)
        {
            return Delete(id);
        }
    }
}
