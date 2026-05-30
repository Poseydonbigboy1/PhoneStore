using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models;
using PhoneStore.Models.Filters;
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

        [HttpGet("filters")]
        public ActionResult<ResultObject<IEnumerable<object>>> GetFilters()
        {
            try
            {
                var filters = _productService.GetFilters();
                return ResultObject<IEnumerable<object>>.Success(filters);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<object>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<Models.ProductsResult>> GetByFilter([FromBody] ProductFilter filter)
        {
            try
            {
                var result = _productService.GetProductsByFilter(filter);
                return ResultObject<Models.ProductsResult>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<Models.ProductsResult>.Error(ex);
            }
        }

        [HttpGet("filter-test")]
        public ActionResult<ResultObject<Models.ProductsResult>> GetTestFilter()
        {
            try
            {
                var filter = new ProductFilter
                {
                    Skip = 0,
                    Take = 20,
                    FilterValues = new List<ProductFilterValue>
                    {
                        new ProductFilterValue
                        {
                            ComponentTitle = "ОЗУ",
                            Value = "8",
                            MatchMode = "equals"
                        },
                        new ProductFilterValue
                        {
                            ComponentTitle = "Цвет",
                            Value = "Natural Titanium",
                            MatchMode = "equals"
                        }
                    }
                };

                var result = _productService.GetProductsByFilter(filter);
                return ResultObject<Models.ProductsResult>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<Models.ProductsResult>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Models.ProductCardViewModel>> GetById([FromRoute] Guid id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                    return ResultObject<Models.ProductCardViewModel>.Error("Товар не найден");

                return ResultObject<Models.ProductCardViewModel>.Success(product);
            }
            catch (Exception ex)
            {
                return ResultObject<Models.ProductCardViewModel>.Error(ex);
            }
        }
        
    }
}