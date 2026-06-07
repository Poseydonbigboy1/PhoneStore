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
    private readonly IWebHostEnvironment  _env;

    public SkuManagementController(SkuManagementService svc, IWebHostEnvironment env)
    {
        _svc = svc;
        _env = env;
    }

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

    // ─── Изображения ───────────────────────────────────────────────────────────

    [HttpGet("{id}/images")]
    public async Task<ActionResult<ResultObject<List<SkuImageViewModel>>>> GetImages([FromRoute] Guid id)
    {
        try { return ResultObject<List<SkuImageViewModel>>.Success(await _svc.GetImagesAsync(id)); }
        catch (Exception ex) { return ResultObject<List<SkuImageViewModel>>.Error(ex); }
    }

    [HttpPost("{id}/images")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB для видео
    public async Task<ActionResult<ResultObject<SkuImageViewModel>>> UploadImage(
        [FromRoute] Guid id, IFormFile file)
    {
        try
        {
            if (file is null || file.Length == 0)
                return BadRequest(ResultObject<SkuImageViewModel>.Error("Файл не выбран"));

            string[] allowedMime = [
                "image/jpeg", "image/png", "image/webp", "image/gif", "image/avif",
                "video/mp4", "video/webm", "video/quicktime", "video/x-msvideo"
            ];
            if (!allowedMime.Contains(file.ContentType.ToLower()))
                return BadRequest(ResultObject<SkuImageViewModel>.Error("Разрешены: jpg, png, webp, gif, mp4, webm, mov"));

            string[] allowedExt = [".jpg", ".jpeg", ".png", ".webp", ".gif", ".avif", ".mp4", ".webm", ".mov", ".avi"];
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
                return BadRequest(ResultObject<SkuImageViewModel>.Error("Неподдерживаемый формат файла"));

            const long maxImageSize = 5  * 1024 * 1024; // 5 MB
            const long maxVideoSize = 50 * 1024 * 1024; // 50 MB
            bool isVideo = file.ContentType.StartsWith("video/");
            if (!isVideo && file.Length > maxImageSize)
                return BadRequest(ResultObject<SkuImageViewModel>.Error("Изображение не должно превышать 5 МБ"));
            if (isVideo && file.Length > maxVideoSize)
                return BadRequest(ResultObject<SkuImageViewModel>.Error("Видео не должно превышать 50 МБ"));

            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsDir = Path.Combine(wwwroot, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            // Абсолютный URL — браузер обращается напрямую к бэку
            var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            var result = await _svc.AddImageAsync(id, url);
            return ResultObject<SkuImageViewModel>.Success(result);
        }
        catch (Exception ex) { return ResultObject<SkuImageViewModel>.Error(ex); }
    }

    [HttpDelete("{id}/images/{productComponentId}")]
    public async Task<ActionResult<ResultObject<bool>>> DeleteImage(
        [FromRoute] Guid id, [FromRoute] Guid productComponentId)
    {
        try
        {
            await _svc.DeleteImageAsync(productComponentId);
            return ResultObject<bool>.Success(true);
        }
        catch (Exception ex) { return ResultObject<bool>.Error(ex); }
    }
}
