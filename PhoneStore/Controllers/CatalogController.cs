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
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("filters")]
        public ActionResult<ResultObject<IEnumerable<object>>> GetFilters()
        {
            try
            {
                var filters = _catalogService.GetFilters();
                return ResultObject<IEnumerable<object>>.Success(filters);
            }
            catch (Exception ex)
            {
                return ResultObject<IEnumerable<object>>.Error(ex);
            }
        }

        [HttpPost("filter")]
        public ActionResult<ResultObject<Models.CatalogResult>> GetByFilter([FromBody] CatalogFilter filter)
        {
            try
            {
                var result = _catalogService.GetProductsByFilter(filter);
                return ResultObject<Models.CatalogResult>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<Models.CatalogResult>.Error(ex);
            }
        }

        [HttpGet("filter-test")]
        public ActionResult<ResultObject<Models.CatalogResult>> GetTestFilter()
        {
            try
            {
                var filter = new CatalogFilter
                {
                    Skip = 0,
                    Take = 20,
                    FilterValues = new List<CatalogFilterValue>
                    {
                        new CatalogFilterValue
                        {
                            ComponentTitle = "ОЗУ",
                            Value = "8",
                            MatchMode = "equals"
                        },
                        new CatalogFilterValue
                        {
                            ComponentTitle = "Цвет",
                            Value = "Natural Titanium",
                            MatchMode = "equals"
                        }
                    }
                };

                var result = _catalogService.GetProductsByFilter(filter);
                return ResultObject<Models.CatalogResult>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<Models.CatalogResult>.Error(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResultObject<Models.ProductCardViewModel>> GetById([FromRoute] Guid id)
        {
            try
            {
                var product = _catalogService.GetProductById(id);
                if (product == null)
                    return ResultObject<Models.ProductCardViewModel>.Error("Товар не найден");

                return ResultObject<Models.ProductCardViewModel>.Success(product);
            }
            catch (Exception ex)
            {
                return ResultObject<Models.ProductCardViewModel>.Error(ex);
            }
        }

        [HttpGet("similar/{skuId}")]
        public ActionResult<ResultObject<List<Models.PoductViewModel>>> GetSimilar(
            [FromRoute] Guid skuId, [FromQuery] int take = 6)
        {
            try
            {
                var result = _catalogService.GetSimilar(skuId, take);
                return ResultObject<List<Models.PoductViewModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<List<Models.PoductViewModel>>.Error(ex);
            }
        }

        [HttpGet("popular")]
        public ActionResult<ResultObject<List<Models.PoductViewModel>>> GetPopular([FromQuery] int take = 10)
        {
            try
            {
                var result = _catalogService.GetPopular(take);
                return ResultObject<List<Models.PoductViewModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<List<Models.PoductViewModel>>.Error(ex);
            }
        }

        [HttpGet("discounted")]
        public ActionResult<ResultObject<List<Models.PoductViewModel>>> GetDiscounted([FromQuery] int take = 10)
        {
            try
            {
                var result = _catalogService.GetDiscounted(take);
                return ResultObject<List<Models.PoductViewModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<List<Models.PoductViewModel>>.Error(ex);
            }
        }

        [HttpPost("batch")]
        public ActionResult<ResultObject<List<Models.PoductViewModel>>> GetBatch([FromBody] List<Guid> skuIds)
        {
            try
            {
                var result = _catalogService.GetBatch(skuIds);
                return ResultObject<List<Models.PoductViewModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResultObject<List<Models.PoductViewModel>>.Error(ex);
            }
        }
    }
}