using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PhoneStore.Data;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Models.RequestModels;
using PhoneStore.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthorizeService _authorizeService;
        private readonly ApplicationContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(AuthorizeService authorizeService, ApplicationContext db)
        {
            _authorizeService = authorizeService;
            _db = db;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                var res = _authorizeService.Login(model);

                if (res.IsSuccess)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        //Secure = true,     // Только через HTTPS
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(60 * 24 * 30)
                    };

                    Response.Cookies.Append("access_token", res.Data, cookieOptions);
                }
                
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true,     // Только через HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(60 * 24 * 30)
                };

                Response.Cookies.Delete("access_token", cookieOptions);

                return Ok(new ResultObject<bool>() { 
                    IsSuccess = true,
                    Data = true
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResultObject<bool>() {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
           

        }

        [HttpPost("getHash")]
        [AllowAnonymous]
        public IActionResult GetHash(LoginModel model)
        {
            try
            {
                var res = _authorizeService.GetHash(model);
                return Ok(new ResultObject<string>() {
                    IsSuccess = true,
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultObject<string>()
                {
                    IsSuccess = true,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("checkAuth")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = userId is not null ? _db.Users.FirstOrDefault(u => u.Id.ToString() == userId) : null;
            var model = new
            {
                Id    = userId,
                Login = User.Identity!.Name,
                Name  = user?.Name,
                Role  = User.FindFirst(ClaimTypes.Role)?.Value,
            };
            return Ok(ResultObject<object>.Success(model));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                if (user is null) return NotFound(ResultObject<bool>.Error("Пользователь не найден"));

                if (!string.IsNullOrWhiteSpace(model.Login) && model.Login != user.Login)
                {
                    if (await _db.Users.AnyAsync(u => u.Login == model.Login))
                        return BadRequest(ResultObject<bool>.Error("Логин уже занят"));
                    user.Login = model.Login;
                }
                if (model.Name is not null) user.Name = model.Name;

                await _db.SaveChangesAsync();
                return Ok(ResultObject<bool>.Success(true));
            }
            catch (Exception ex) { return BadRequest(ResultObject<bool>.Error(ex.Message)); }
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                if (user is null) return NotFound(ResultObject<bool>.Error("Пользователь не найден"));

                var verify = _hasher.VerifyHashedPassword(user, user.Password, model.OldPassword);
                if (verify == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                    return BadRequest(ResultObject<bool>.Error("Неверный текущий пароль"));

                user.Password = _hasher.HashPassword(user, model.NewPassword);
                await _db.SaveChangesAsync();
                return Ok(ResultObject<bool>.Success(true));
            }
            catch (Exception ex) { return BadRequest(ResultObject<bool>.Error(ex.Message)); }
        }
    }
}
