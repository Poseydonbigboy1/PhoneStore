using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models.ViewModels;
using PhoneStore.Services;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "MANAGER")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _svc;

    public DashboardController(DashboardService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<ResultObject<DashboardViewModel>>> Get()
    {
        try
        {
            var data = await _svc.GetDashboardAsync();
            return ResultObject<DashboardViewModel>.Success(data);
        }
        catch (Exception ex) { return ResultObject<DashboardViewModel>.Error(ex); }
    }
}
