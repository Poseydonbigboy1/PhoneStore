using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;
using PhoneStore.Models.RequestModels;
using PhoneStore.Models.ViewModels;
using PhoneStore.Services;
using System.Security.Claims;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    private Guid GetUserId()
    {
        var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Не удалось определить пользователя");
        return Guid.Parse(raw);
    }

    [HttpGet]
    public async Task<ActionResult<ResultObject<List<CartItemViewModel>>>> GetCart()
    {
        try
        {
            var items = await _cartService.GetCartAsync(GetUserId());
            return ResultObject<List<CartItemViewModel>>.Success(items);
        }
        catch (Exception ex) { return ResultObject<List<CartItemViewModel>>.Error(ex); }
    }

    [HttpPost]
    public async Task<ActionResult<ResultObject<bool>>> AddItem([FromBody] CartAddRequest req)
    {
        try
        {
            await _cartService.AddItemAsync(GetUserId(), req.SkuId, req.Quantity);
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }

    [HttpPut("{skuId}")]
    public async Task<ActionResult<ResultObject<bool>>> UpdateItem([FromRoute] Guid skuId, [FromBody] CartUpdateRequest req)
    {
        try
        {
            await _cartService.UpdateItemAsync(GetUserId(), skuId, req.Quantity);
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }

    [HttpDelete("{skuId}")]
    public async Task<ActionResult<ResultObject<bool>>> RemoveItem([FromRoute] Guid skuId)
    {
        try
        {
            await _cartService.RemoveItemAsync(GetUserId(), skuId);
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }

    [HttpPost("merge")]
    public async Task<ActionResult<ResultObject<bool>>> Merge([FromBody] List<CartMergeItem> items)
    {
        try
        {
            await _cartService.MergeAsync(GetUserId(), items);
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }
}
