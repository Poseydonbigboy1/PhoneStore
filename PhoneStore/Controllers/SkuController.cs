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
    public class SkuController : ControllerBase
    {
        private readonly SkuService _skuService;

        public SkuController(SkuService skuService)
        {
            _skuService = skuService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<Sku>>> GetAll()
        {
            try
            {
                var data = _skuService.GetAllSkus();
                return ResultObject<IEnumerable<Sku>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<Sku>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Sku>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _skuService.GetById(id);
                if (item == null) return ResultObject<Sku>.Error("Sku not found");
                return ResultObject<Sku>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<Sku>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<Sku>> Create([FromBody] Sku sku)
        {
            try
            {
                var created = _skuService.CreateSku(sku);
                return ResultObject<Sku>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<Sku>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<Sku>> Update([FromBody] Sku sku)
        {
            try
            {
                var updated = _skuService.UpdateSku(sku);
                if (updated == null) return ResultObject<Sku>.Error("Sku not found");
                return ResultObject<Sku>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<Sku>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _skuService.DeleteSku(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
