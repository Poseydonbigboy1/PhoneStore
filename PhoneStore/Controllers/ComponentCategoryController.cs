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
    public class ComponentCategoryController : ControllerBase
    {
        private readonly ComponentCategoryService _categoryService;

        public ComponentCategoryController(ComponentCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<ComponentCategory>>> GetAll()
        {
            try
            {
                var data = _categoryService.GetAllCategories();
                return ResultObject<IEnumerable<ComponentCategory>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<ComponentCategory>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<ComponentCategory>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _categoryService.GetById(id);
                if (item == null) return ResultObject<ComponentCategory>.Error("Category not found");
                return ResultObject<ComponentCategory>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<ComponentCategory>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<ComponentCategory>> Create([FromBody] ComponentCategory category)
        {
            try
            {
                var created = _categoryService.CreateCategory(category);
                return ResultObject<ComponentCategory>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<ComponentCategory>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<ComponentCategory>> Update([FromBody] ComponentCategory category)
        {
            try
            {
                var updated = _categoryService.UpdateCategory(category);
                if (updated == null) return ResultObject<ComponentCategory>.Error("Category not found");
                return ResultObject<ComponentCategory>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<ComponentCategory>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _categoryService.DeleteCategory(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
