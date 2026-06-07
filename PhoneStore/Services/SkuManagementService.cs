using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models.ViewModels;

namespace PhoneStore.Services;

public class SkuManagementService
{
    private readonly ApplicationContext _db;

    public SkuManagementService(ApplicationContext db) => _db = db;

    public async Task<List<SkuManagementViewModel>> GetAllAsync()
    {
        var skus = await _db.Skus
            .Include(s => s.Product).ThenInclude(p => p!.Brand)
            .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                .ThenInclude(c => c!.ComponentCategory)
            .OrderBy(s => s.Product!.Title)
            .ToListAsync();

        return skus.Select(MapToViewModel).ToList();
    }

    public async Task<SkuManagementViewModel?> GetByIdAsync(Guid id)
    {
        var sku = await _db.Skus
            .Include(s => s.Product).ThenInclude(p => p!.Brand)
            .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                .ThenInclude(c => c!.ComponentCategory)
            .FirstOrDefaultAsync(s => s.Id == id);

        return sku is null ? null : MapToViewModel(sku);
    }

    public async Task<SkuManagementViewModel> UpsertAsync(SkuUpsertRequest req)
    {
        Sku sku;

        if (req.Id.HasValue)
        {
            sku = await _db.Skus
                .Include(s => s.ProductComponents)
                .FirstOrDefaultAsync(s => s.Id == req.Id.Value)
                ?? throw new Exception("SKU не найден");

            sku.ProductId = req.ProductId;
            sku.Price     = req.Price;
            sku.Discount  = req.Discount;
            sku.Amount    = req.Amount;

            // Удаляем компоненты которых нет в запросе
            var keepIds = req.Components
                .Where(c => c.ProductComponentId.HasValue)
                .Select(c => c.ProductComponentId!.Value)
                .ToHashSet();
            var toRemove = sku.ProductComponents.Where(pc => !keepIds.Contains(pc.Id)).ToList();
            _db.ProductComponents.RemoveRange(toRemove);
        }
        else
        {
            sku = new Sku
            {
                ProductId = req.ProductId,
                Price     = req.Price,
                Discount  = req.Discount,
                Amount    = req.Amount,
            };
            _db.Skus.Add(sku);
            await _db.SaveChangesAsync(); // получаем Id
        }

        // Обновляем / добавляем компоненты
        foreach (var comp in req.Components)
        {
            if (comp.ProductComponentId.HasValue)
            {
                var existing = await _db.ProductComponents.FindAsync(comp.ProductComponentId.Value);
                if (existing is not null)
                    existing.Value = comp.Value;
            }
            else
            {
                _db.ProductComponents.Add(new ProductComponent
                {
                    SkuId       = sku.Id,
                    ComponentId = comp.ComponentId,
                    Value       = comp.Value,
                });
            }
        }

        await _db.SaveChangesAsync();
        return (await GetByIdAsync(sku.Id))!;
    }

    public async Task DeleteAsync(Guid id)
    {
        var sku = await _db.Skus.Include(s => s.ProductComponents)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new Exception("SKU не найден");

        _db.ProductComponents.RemoveRange(sku.ProductComponents);
        _db.Skus.Remove(sku);
        await _db.SaveChangesAsync();
    }

    // ─── Изображения ───────────────────────────────────────────────────────────

    public async Task<List<SkuImageViewModel>> GetImagesAsync(Guid skuId)
    {
        var rows = await _db.ProductComponents
            .Include(pc => pc.Component)
            .Where(pc => pc.SkuId == skuId && pc.Component!.DataType == EDataType.IMAGE)
            .OrderBy(pc => pc.Id)
            .ToListAsync();

        return rows.Select(pc => new SkuImageViewModel
        {
            ProductComponentId = pc.Id,
            Url = pc.Value?.ToString() ?? string.Empty
        }).ToList();
    }

    public async Task<SkuImageViewModel> AddImageAsync(Guid skuId, string url)
    {
        var imageComponent = await GetOrCreateImageComponentAsync();

        var pc = new ProductComponent
        {
            SkuId       = skuId,
            ComponentId = imageComponent.Id,
            Filtering   = false,
        };
        pc.Value = url;   // setter сериализует в ValueJson

        _db.ProductComponents.Add(pc);
        await _db.SaveChangesAsync();

        return new SkuImageViewModel { ProductComponentId = pc.Id, Url = url };
    }

    public async Task DeleteImageAsync(Guid productComponentId)
    {
        var pc = await _db.ProductComponents
            .Include(pc => pc.Component)
            .FirstOrDefaultAsync(pc => pc.Id == productComponentId
                                    && pc.Component!.DataType == EDataType.IMAGE)
            ?? throw new Exception("Изображение не найдено");

        _db.ProductComponents.Remove(pc);
        await _db.SaveChangesAsync();
    }

    /// <summary>Находит компонент с DataType=IMAGE или создаёт системный, если его нет.</summary>
    private async Task<Component> GetOrCreateImageComponentAsync()
    {
        var comp = await _db.Components.FirstOrDefaultAsync(c => c.DataType == EDataType.IMAGE);
        if (comp != null) return comp;

        var category = await _db.ComponentCategories.FirstOrDefaultAsync()
            ?? throw new Exception("Нет категорий компонентов — создайте хотя бы одну");

        comp = new Component
        {
            Id                  = Guid.NewGuid(),
            Title               = "Изображение",
            Description         = "Изображение товара",
            DataType            = EDataType.IMAGE,
            ComponentCategoryId = category.Id,
        };
        _db.Components.Add(comp);
        await _db.SaveChangesAsync();
        return comp;
    }

    private static SkuManagementViewModel MapToViewModel(Sku s) => new()
    {
        Id           = s.Id,
        ProductId    = s.ProductId,
        ProductTitle = s.Product?.Title ?? string.Empty,
        BrandTitle   = s.Product?.Brand?.Title ?? string.Empty,
        Price        = s.Price,
        Discount     = s.Discount,
        FinalPrice   = s.Discount > 0 ? s.Price * (1 - s.Discount / 100) : s.Price,
        Amount       = s.Amount,
        Components   = s.ProductComponents.Select(pc => new SkuComponentView
        {
            ProductComponentId = pc.Id,
            ComponentId        = pc.ComponentId,
            ComponentTitle     = pc.Component?.Title ?? string.Empty,
            CategoryTitle      = pc.Component?.ComponentCategory?.Title ?? string.Empty,
            Value              = pc.Value?.ToString() ?? string.Empty,
        }).ToList(),
    };
}
