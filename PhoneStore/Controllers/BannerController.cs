using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Helpers;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannerController : ControllerBase
{
    private readonly ApplicationContext _db;

    public BannerController(ApplicationContext db) => _db = db;

    // ─── Публичный эндпоинт: только активные, отсортированные ───────────────
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ResultObject<List<Banner>>>> GetActive()
    {
        var banners = await _db.Banners
            .Where(b => b.IsActive)
            .OrderBy(b => b.SortOrder)
            .ToListAsync();
        return ResultObject<List<Banner>>.Success(banners);
    }

    // ─── Менеджер: все баннеры ───────────────────────────────────────────────
    [HttpGet("all")]
    [Authorize(Roles = "MANAGER")]
    public async Task<ActionResult<ResultObject<List<Banner>>>> GetAll()
    {
        var banners = await _db.Banners
            .OrderBy(b => b.SortOrder)
            .ToListAsync();
        return ResultObject<List<Banner>>.Success(banners);
    }

    // ─── Создать баннер ──────────────────────────────────────────────────────
    [HttpPost]
    [Authorize(Roles = "MANAGER")]
    public async Task<ActionResult<ResultObject<Banner>>> Create([FromBody] BannerUpsertModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ImageUrl))
            return BadRequest(ResultObject<Banner>.Error("URL изображения обязателен"));

        var maxOrder = await _db.Banners.MaxAsync(b => (int?)b.SortOrder) ?? -1;

        var banner = new Banner
        {
            ImageUrl  = model.ImageUrl,
            Link      = model.Link ?? string.Empty,
            SortOrder = maxOrder + 1,
            IsActive  = model.IsActive ?? true,
        };

        _db.Banners.Add(banner);
        await _db.SaveChangesAsync();
        return Ok(ResultObject<Banner>.Success(banner));
    }

    // ─── Обновить баннер ─────────────────────────────────────────────────────
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "MANAGER")]
    public async Task<ActionResult<ResultObject<bool>>> Update(
        Guid id, [FromBody] BannerUpsertModel model)
    {
        var banner = await _db.Banners.FindAsync(id);
        if (banner is null)
            return NotFound(ResultObject<bool>.Error("Баннер не найден"));

        if (!string.IsNullOrWhiteSpace(model.ImageUrl)) banner.ImageUrl  = model.ImageUrl;
        if (model.Link     is not null)                  banner.Link      = model.Link;
        if (model.IsActive is not null)                  banner.IsActive  = model.IsActive.Value;
        if (model.SortOrder is not null)                 banner.SortOrder = model.SortOrder.Value;

        await _db.SaveChangesAsync();
        return Ok(ResultObject<bool>.Success(true));
    }

    // ─── Удалить баннер ──────────────────────────────────────────────────────
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "MANAGER")]
    public async Task<ActionResult<ResultObject<bool>>> Delete(Guid id)
    {
        var banner = await _db.Banners.FindAsync(id);
        if (banner is null)
            return NotFound(ResultObject<bool>.Error("Баннер не найден"));

        _db.Banners.Remove(banner);
        await _db.SaveChangesAsync();
        return Ok(ResultObject<bool>.Success(true));
    }

    // ─── Переставить порядок (массовое обновление SortOrder) ─────────────────
    [HttpPut("reorder")]
    [Authorize(Roles = "MANAGER")]
    public async Task<ActionResult<ResultObject<bool>>> Reorder([FromBody] List<BannerOrderItem> items)
    {
        foreach (var item in items)
        {
            await _db.Banners
                .Where(b => b.Id == item.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.SortOrder, item.SortOrder));
        }
        return Ok(ResultObject<bool>.Success(true));
    }
}

public class BannerUpsertModel
{
    public string? ImageUrl  { get; set; }
    public string? Link      { get; set; }
    public bool?   IsActive  { get; set; }
    public int?    SortOrder { get; set; }
}

public class BannerOrderItem
{
    public Guid Id        { get; set; }
    public int  SortOrder { get; set; }
}
