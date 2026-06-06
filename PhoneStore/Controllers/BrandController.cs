using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
using PhoneStore.Data;
using PhoneStore.Services;

namespace PhoneStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly BrandService _brandService;

        public BrandController(BrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<Brand>>> GetAll()
        {
            try
            {
                var data = _brandService.GetAllBrands();
                return ResultObject<IEnumerable<Brand>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<Brand>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<FilterResult<Brand>>> GetByFilter([FromBody] BrandFilter filter)
        {
            try
            {
                var data = _brandService.GetDataByFilter(filter);
                return ResultObject<FilterResult<Brand>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<FilterResult<Brand>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Brand>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _brandService.GetById(id);
                if (item == null) return ResultObject<Brand>.Error("Brand not found");
                return ResultObject<Brand>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<Brand>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<Brand>> Create([FromBody] Brand brand)
        {
            try
            {
                var created = _brandService.CreateBrand(brand);
                return ResultObject<Brand>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<Brand>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<Brand>> Update([FromBody] Brand brand)
        {
            try
            {
                var updated = _brandService.UpdateBrand(brand);
                if (updated == null) return ResultObject<Brand>.Error("Brand not found");
                return ResultObject<Brand>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<Brand>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _brandService.DeleteBrand(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
