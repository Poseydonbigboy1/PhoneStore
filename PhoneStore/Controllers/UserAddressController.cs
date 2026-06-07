using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Helpers;
using System.Security.Claims;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserAddressController : ControllerBase
{
    private readonly ApplicationContext _db;

    public UserAddressController(ApplicationContext db) => _db = db;

    private Guid CurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<ActionResult<ResultObject<List<UserAddress>>>> GetAll()
    {
        try
        {
            var uid = CurrentUserId();
            var list = await _db.UserAddresses
                .Where(a => a.UserId == uid)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.Label)
                .ToListAsync();
            return ResultObject<List<UserAddress>>.Success(list);
        }
        catch (Exception ex) { return ResultObject<List<UserAddress>>.Error(ex); }
    }

    [HttpPost]
    public async Task<ActionResult<ResultObject<UserAddress>>> Create([FromBody] UserAddress model)
    {
        try
        {
            var uid = CurrentUserId();
            var address = new UserAddress
            {
                UserId    = uid,
                Label     = model.Label,
                Address   = model.Address,
                IsDefault = model.IsDefault,
            };

            if (model.IsDefault)
                await _db.UserAddresses
                    .Where(a => a.UserId == uid)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));

            _db.UserAddresses.Add(address);
            await _db.SaveChangesAsync();
            return ResultObject<UserAddress>.Success(address);
        }
        catch (Exception ex) { return ResultObject<UserAddress>.Error(ex); }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ResultObject<UserAddress>>> Update([FromRoute] Guid id, [FromBody] UserAddress model)
    {
        try
        {
            var uid = CurrentUserId();
            var address = await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == uid);
            if (address is null) return ResultObject<UserAddress>.Error("Адрес не найден");

            address.Label   = model.Label;
            address.Address = model.Address;

            if (model.IsDefault && !address.IsDefault)
            {
                await _db.UserAddresses
                    .Where(a => a.UserId == uid)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));
                address.IsDefault = true;
            }

            await _db.SaveChangesAsync();
            return ResultObject<UserAddress>.Success(address);
        }
        catch (Exception ex) { return ResultObject<UserAddress>.Error(ex); }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResultObject<bool>>> Delete([FromRoute] Guid id)
    {
        try
        {
            var uid = CurrentUserId();
            var address = await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == uid);
            if (address is null) return ResultObject<bool>.Error("Адрес не найден");
            _db.UserAddresses.Remove(address);
            await _db.SaveChangesAsync();
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }
}
