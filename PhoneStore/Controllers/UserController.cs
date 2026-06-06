using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Services;

namespace PhoneStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<User>>> GetAll()
        {
            try
            {
                var data = _userService.GetAllUsers();
                return ResultObject<IEnumerable<User>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<User>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<FilterResult<User>>> GetByFilter([FromBody] UserFilter filter)
        {
            try
            {
                var data = _userService.GetDataByFilter(filter);
                return ResultObject<FilterResult<User>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<FilterResult<User>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<User>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _userService.GetById(id);
                if (item == null) return ResultObject<User>.Error("User not found");
                return ResultObject<User>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<User>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<User>> Create([FromBody] User user)
        {
            try
            {
                var created = _userService.CreateUser(user);
                return ResultObject<User>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<User>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<User>> Update([FromBody] User user)
        {
            try
            {
                var updated = _userService.UpdateUser(user);
                if (updated == null) return ResultObject<User>.Error("User not found");
                return ResultObject<User>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<User>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _userService.DeleteUser(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
