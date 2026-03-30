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

        [HttpPost("filter")]
        public ActionResult<ResultObject<IEnumerable<PoductViewModel>>> GetByFilter([FromBody] ProductFilter filter)
        {
            try
            {
                var products = _productService.GetProductsByFilter(filter);
                return ResultObject<IEnumerable<PoductViewModel>>.Success(products);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<PoductViewModel>>.Error(ex);
            }
        }

        [HttpGet("filter-test")]
        public ActionResult<ResultObject<IEnumerable<PoductViewModel>>> GetTestFilter()
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

                var products = _productService.GetProductsByFilter(filter);
                return ResultObject<IEnumerable<PoductViewModel>>.Success(products);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<PoductViewModel>>.Error(ex);
            }
        }
        
    }
}