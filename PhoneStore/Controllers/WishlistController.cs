using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Helpers;
using PhoneStore.Models;
using System.Security.Claims;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly ApplicationContext _db;

    public WishlistController(ApplicationContext db) => _db = db;

    private Guid CurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private async Task<bool> UserExistsAsync(Guid userId) =>
        await _db.Users.AnyAsync(u => u.Id == userId);

    [HttpGet]
    public async Task<ActionResult<ResultObject<List<PoductViewModel>>>> GetWishlist()
    {
        try
        {
            var uid = CurrentUserId();
            var items = await _db.Wishlists
                .Where(w => w.UserId == uid)
                .Include(w => w.Sku).ThenInclude(s => s!.Product).ThenInclude(p => p!.Brand)
                .Include(w => w.Sku).ThenInclude(s => s!.ProductComponents).ThenInclude(pc => pc.Component)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();

            var result = items.Select(w =>
            {
                var sku = w.Sku!;
                var product = sku.Product!;
                var imageUrl = sku.ProductComponents
                    .FirstOrDefault(pc => pc.Component?.DataType == EDataType.IMAGE)
                    ?.Value?.ToString();

                return new PoductViewModel
                {
                    SkuId       = sku.Id,
                    ProductId   = product.Id,
                    Title       = product.Title,
                    BrandTitle  = product.Brand?.Title ?? string.Empty,
                    Price       = (double)sku.Price,
                    Discount    = (double)sku.Discount,
                    Amount      = (int)sku.Amount,
                    ImageUrl    = imageUrl,
                };
            }).ToList();

            return ResultObject<List<PoductViewModel>>.Success(result);
        }
        catch (Exception ex) { return ResultObject<List<PoductViewModel>>.Error(ex); }
    }

    [HttpGet("ids")]
    public async Task<ActionResult<ResultObject<List<Guid>>>> GetWishlistIds()
    {
        try
        {
            var uid = CurrentUserId();
            var ids = await _db.Wishlists
                .Where(w => w.UserId == uid)
                .Select(w => w.SkuId)
                .ToListAsync();
            return ResultObject<List<Guid>>.Success(ids);
        }
        catch (Exception ex) { return ResultObject<List<Guid>>.Error(ex); }
    }

    [HttpPost("{skuId}")]
    public async Task<ActionResult<ResultObject<bool>>> Add([FromRoute] Guid skuId)
    {
        try
        {
            var uid = CurrentUserId();
            if (!await UserExistsAsync(uid))
                return Unauthorized(ResultObject<bool>.Error("Сессия устарела. Выполните вход заново."));

            if (!await _db.Wishlists.AnyAsync(w => w.UserId == uid && w.SkuId == skuId))
            {
                _db.Wishlists.Add(new Wishlist { UserId = uid, SkuId = skuId });
                await _db.SaveChangesAsync();
            }
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }

    [HttpDelete("{skuId}")]
    public async Task<ActionResult<ResultObject<bool>>> Remove([FromRoute] Guid skuId)
    {
        try
        {
            var uid = CurrentUserId();
            var item = await _db.Wishlists.FirstOrDefaultAsync(w => w.UserId == uid && w.SkuId == skuId);
            if (item is not null)
            {
                _db.Wishlists.Remove(item);
                await _db.SaveChangesAsync();
            }
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }
}
