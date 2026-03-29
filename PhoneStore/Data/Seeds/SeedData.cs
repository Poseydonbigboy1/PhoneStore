using Microsoft.AspNetCore.Identity;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<User> CreateUsers() 
        {
            var passwordHasher = new PasswordHasher<User>();
            
            var users = new List<User>() {
                new User()
                {
                    Id = Guid.NewGuid(),
                    Name = "Вася Пупкин",
                    Login = "test",
                    Roles = ERole.CUSTOMER
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Name = "Петя Шишкин",
                    Login = "test2",
                    Roles = ERole.CUSTOMER
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Name = "Менеджер",
                    Login = "man",
                    Roles = ERole.MANAGER
                },
            };

            users[0].Password = passwordHasher.HashPassword(users[0], "test");
            users[1].Password = passwordHasher.HashPassword(users[1], "test2");
            users[2].Password = passwordHasher.HashPassword(users[2], "man");

            return users;
        }
    }
}
