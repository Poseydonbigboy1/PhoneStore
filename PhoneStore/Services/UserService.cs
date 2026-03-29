using PhoneStore.Data;
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

        public List<User> GetDataByFilter(UserFilter filter)
        {

            return _db.Users
                .Where(user => filter.Login != null ? user.Login == filter.Login : true)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();

        }
    }
}
