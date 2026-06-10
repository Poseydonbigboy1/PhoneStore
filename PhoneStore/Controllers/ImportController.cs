using System;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models.Import;
using PhoneStore.Services;

namespace PhoneStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly PhoneImportService _importService;

        public ImportController(PhoneImportService importService)
        {
            _importService = importService;
        }

        /// <summary>
        /// Импортирует телефоны из статического датасета: генерирует 500+ SKU со всеми
        /// компонентами и ценами в рублях. Идемпотентно — уже существующие товары пропускаются.
        /// Предназначен для разового запуска после создания БД (демо-наполнение).
        /// </summary>
        [HttpPost("phones")]
        public ActionResult<ResultObject<ImportSummary>> ImportPhones()
        {
            try
            {
                var summary = _importService.Import();
                return ResultObject<ImportSummary>.Success(summary);
            }
            catch (Exception ex)
            {
                return ResultObject<ImportSummary>.Error(ex);
            }
        }
    }
}
