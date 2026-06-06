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
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<ResultObject<IEnumerable<Product>>> GetAll()
        {
            try
            {
                var data = _productService.GetAllProducts();
                return ResultObject<IEnumerable<Product>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<Product>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<FilterResult<Product>>> GetByFilter([FromBody] ProductFilter filter)
        {
            try
            {
                var data = _productService.GetDataByFilter(filter);
                return ResultObject<FilterResult<Product>>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultObject<FilterResult<Product>>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Product>> GetById([FromRoute] Guid id)
        {
            try
            {
                var item = _productService.GetById(id);
                if (item == null) return ResultObject<Product>.Error("Product not found");
                return ResultObject<Product>.Success(item);
            }
            catch (Exception ex)
            {
                return ResultObject<Product>.Error(ex);
            }
        }

        [HttpPost]
        public ActionResult<ResultObject<Product>> Create([FromBody] Product product)
        {
            try
            {
                var created = _productService.CreateProduct(product);
                return ResultObject<Product>.Success(created);
            }
            catch (Exception ex)
            {
                return ResultObject<Product>.Error(ex);
            }
        }

        [HttpPut]
        public ActionResult<ResultObject<Product>> Update([FromBody] Product product)
        {
            try
            {
                var updated = _productService.UpdateProduct(product);
                if (updated == null) return ResultObject<Product>.Error("Product not found");
                return ResultObject<Product>.Success(updated);
            }
            catch (Exception ex)
            {
                return ResultObject<Product>.Error(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResultObject<bool>> Delete([FromRoute] Guid id)
        {
            try
            {
                var ok = _productService.DeleteProduct(id);
                return ResultObject<bool>.Success(ok);
            }
            catch (Exception ex)
            {
                return ResultObject<bool>.Error(ex);
            }
        }
    }
}
