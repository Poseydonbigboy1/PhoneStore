using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;

namespace PhoneStore.Services
{
    public class UserService
    {

        private readonly ApplicationContext _db;

        public UserService(ApplicationContext db)
        {
            _db = db;
        }
        public User? GetById(Guid id)
        {
            return _db.Users
                    .FirstOrDefault(u => u.Id == id);

        }

        public FilterResult<User> GetDataByFilter(UserFilter filter)
        {
            var query = _db.Users
                .Where(user => filter.Login != null ? user.Login == filter.Login : true);
            
            var total = query.Count();
            var items = query
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();

            return new FilterResult<User> { Items = items, Total = total };
        }
    }
}
