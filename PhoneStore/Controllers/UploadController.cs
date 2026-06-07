using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Helpers;

namespace PhoneStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "MANAGER")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>Загрузить изображение товара. Возвращает URL для сохранения в ValueJson компонента.</summary>
    [HttpPost("image")]
    public async Task<ActionResult<ResultObject<string>>> UploadImage(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ResultObject<string>.Error("Файл не выбран"));

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest(ResultObject<string>.Error("Разрешены только изображения"));

        const long maxSize = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxSize)
            return BadRequest(ResultObject<string>.Error("Размер файла не должен превышать 5 МБ"));

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowed.Contains(ext))
            return BadRequest(ResultObject<string>.Error("Разрешённые форматы: jpg, jpeg, png, webp, gif"));

        var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsDir = Path.Combine(wwwroot, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(ResultObject<string>.Success(url));
    }
}
