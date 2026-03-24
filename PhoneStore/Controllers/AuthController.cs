using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Services;
using System.Security.Claims;

namespace PhoneStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthorizeService _authorizeService;
        public AuthController(AuthorizeService authorizeService) 
        { 
            _authorizeService = authorizeService;
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

        // TODO: аменить на получение провфиля
        [HttpGet("checkAuth")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var model = new
            {
                Login = User.Identity.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            };
            
            return Ok(ResultObject<object>.Success(model));
        }
        
    }
}
