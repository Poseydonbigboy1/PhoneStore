using Microsoft.AspNetCore.Identity;
using PhoneStore.Data;
using PhoneStore.Helpers;
using PhoneStore.Models;

namespace PhoneStore.Services
{
    public class AuthorizeService
    {
        private readonly PasswordHasher<User> _hasher = new();
        private readonly IConfiguration _config;
        public AuthorizeService(IConfiguration config) 
        {
            _config = config;
        }

        public ResultObject<string> Login(LoginModel model)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    var user  = db.Users
                        .FirstOrDefault(f => f.Login == model.Login);

                    if (user != null)
                    { 
                        var isPasswordValid = _hasher.VerifyHashedPassword(user,user.Password, model.Password);
                        if (isPasswordValid == PasswordVerificationResult.Success)
                        {
                            var signingKey = _config.GetSection("Settings").GetValue<string>("SigningKey");
                            var token = JwtService.GenerateToken(user.Login, user.Roles.ToString(), signingKey);                            

                            return new ResultObject<string>() { IsSuccess = true, Data = token};
                        } else
                        {
                            return new ResultObject<string>() { IsSuccess = false, Message = "Не верный пароль" };
                        }
                    } else
                    {
                        return new ResultObject<string>() { IsSuccess = false, Message = "Не верный логин" };
                    }
                }
            }
            catch (Exception ex) 
            {
                return new ResultObject<string>() { IsSuccess = false, Message = ex.Message };
            }
        }

        public void Registration(RegisterModel model) { 
            try
            {
                using (var db = new ApplicationContext())
                {
                    var user = new User()
                    {
                        Login = model.Login,
                        Name = model.Name,
                        Roles = ERole.CUSTOMER,
                    };

                    user.Password = _hasher.HashPassword(user, model.Password);

                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }
            catch (Exception ex) {
                throw new Exception("Ошибка регистрации");
            }
        }

        public string GetHash(LoginModel model)
        {
            using (var db = new ApplicationContext())
            {
                var user = db.Users
                       .FirstOrDefault(f => f.Login == model.Login);

                return _hasher.HashPassword(user, model.Password);
            }
                
        }
    }
}
