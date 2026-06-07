using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models.ViewModels;
using PhoneStore.Services;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "MANAGER")]
public class SkuManagementController : ControllerBase
{
    private readonly SkuManagementService _svc;

    public SkuManagementController(SkuManagementService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<ResultObject<List<SkuManagementViewModel>>>> GetAll()
    {
        try { return ResultObject<List<SkuManagementViewModel>>.Success(await _svc.GetAllAsync()); }
        catch (Exception ex) { return ResultObject<List<SkuManagementViewModel>>.Error(ex); }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResultObject<SkuManagementViewModel>>> GetById([FromRoute] Guid id)
    {
        try
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null
                ? ResultObject<SkuManagementViewModel>.Error("SKU не найден")
                : ResultObject<SkuManagementViewModel>.Success(item);
        }
        catch (Exception ex) { return ResultObject<SkuManagementViewModel>.Error(ex); }
    }

    [HttpPost]
    public async Task<ActionResult<ResultObject<SkuManagementViewModel>>> Upsert([FromBody] SkuUpsertRequest req)
    {
        try { return ResultObject<SkuManagementViewModel>.Success(await _svc.UpsertAsync(req)); }
        catch (Exception ex) { return ResultObject<SkuManagementViewModel>.Error(ex); }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResultObject<bool>>> Delete([FromRoute] Guid id)
    {
        try { await _svc.DeleteAsync(id); return ResultObject<bool>.Success(true); }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }
}
