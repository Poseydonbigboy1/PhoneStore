using PhoneStore.Data;
using PhoneStore.Models.Filters;

namespace PhoneStore.Services
{
    public class UserService
    {
        public User? GetById(Guid id) 
        {
            using (ApplicationContext db = new ApplicationContext()) 
            {
                return db.Users
                    .FirstOrDefault(u => u.Id == id);
            }
        }

        public List<User> GetDataByFilter(UserFilter filter) 
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Users
                    .Where(user => filter.Login != null ? user.Login == filter.Login : true)
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .ToList();
            }
        }
    }
}
