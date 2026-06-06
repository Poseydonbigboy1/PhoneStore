using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Data;
using PhoneStore.Services;

namespace PhoneStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentController : ControllerBase
    {
        private readonly ComponentService _componentService;

        public ComponentController(ComponentService componentService)
        {
            _componentService = componentService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<Component>>> GetAll()
        {
            try
            {
                var data = _componentService.GetAllComponents();
                return ResultObject<IEnumerable<Component>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<Component>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Component>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _componentService.GetById(id);
                if (item == null) return ResultObject<Component>.Error("Component not found");
                return ResultObject<Component>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<Component>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<Component>> Create([FromBody] Component component)
        {
            try
            {
                var created = _componentService.CreateComponent(component);
                return ResultObject<Component>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<Component>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<Component>> Update([FromBody] Component component)
        {
            try
            {
                var updated = _componentService.UpdateComponent(component);
                if (updated == null) return ResultObject<Component>.Error("Component not found");
                return ResultObject<Component>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<Component>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _componentService.DeleteComponent(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
