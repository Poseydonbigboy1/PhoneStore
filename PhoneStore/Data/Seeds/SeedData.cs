namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<User> CreateUsers() 
        {
            return new List<User>() {
                new User()
                {
                    Name = "Вася Пупкин",
                    Login = "test",
                    Password = "test",
                    Roles = ERole.CUSTOMER
                },
                new User()
                {
                    Name = "Петя Шишкин",
                    Login = "test2",
                    Password = "test2",
                    Roles = ERole.CUSTOMER
                },
                new User()
                {
                    Name = "Менеджер",
                    Login = "man",
                    Password = "man",
                    Roles = ERole.MANAGER
                },
            };
        }
    }
}
