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
    public class ProductComponentController : ControllerBase
    {
        private readonly ProductComponentService _productComponentService;

        public ProductComponentController(ProductComponentService productComponentService)
        {
            _productComponentService = productComponentService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<ProductComponent>>> GetAll()
        {
            try
            {
                var data = _productComponentService.GetAllProductComponents();
                return ResultObject<IEnumerable<ProductComponent>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<ProductComponent>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<FilterResult<ProductComponent>>> GetByFilter([FromBody] ProductComponentFilter filter)
        {
            try
            {
                var data = _productComponentService.GetDataByFilter(filter);
                return ResultObject<FilterResult<ProductComponent>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<FilterResult<ProductComponent>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<ProductComponent>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _productComponentService.GetById(id);
                if (item == null) return ResultObject<ProductComponent>.Error("ProductComponent not found");
                return ResultObject<ProductComponent>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<ProductComponent>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<ProductComponent>> Create([FromBody] ProductComponent productComponent)
        {
            try
            {
                var created = _productComponentService.CreateProductComponent(productComponent);
                return ResultObject<ProductComponent>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<ProductComponent>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<ProductComponent>> Update([FromBody] ProductComponent productComponent)
        {
            try
            {
                var updated = _productComponentService.UpdateProductComponent(productComponent);
                if (updated == null) return ResultObject<ProductComponent>.Error("ProductComponent not found");
                return ResultObject<ProductComponent>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<ProductComponent>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _productComponentService.DeleteProductComponent(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
