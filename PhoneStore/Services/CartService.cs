using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models.RequestModels;
using PhoneStore.Models.ViewModels;

namespace PhoneStore.Services;

public class CartService
{
    private readonly ApplicationContext _db;

    public CartService(ApplicationContext db)
    {
        _db = db;
    }

    public async Task<List<CartItemViewModel>> GetCartAsync(Guid userId)
    {
        return await _db.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Sku).ThenInclude(s => s!.Product)
            .Select(c => new CartItemViewModel
            {
                SkuId       = c.SkuId,
                Quantity    = c.Quantity,
                ProductTitle = c.Sku!.Product!.Title,
                Price       = (decimal)c.Sku.Price,
                Discount    = (int)c.Sku.Discount,
                FinalPrice  = Math.Round((decimal)c.Sku.Price * (1m - (decimal)c.Sku.Discount / 100m), 2),
                Amount      = (int)c.Sku.Amount,
            })
            .ToListAsync();
    }

    public async Task AddItemAsync(Guid userId, Guid skuId, int quantity)
    {
        var sku = await _db.Skus.FindAsync(skuId)
            ?? throw new Exception("SKU не найден");

        var existing = await _db.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId && c.SkuId == skuId);

        var newQty = (existing?.Quantity ?? 0) + quantity;
        if (sku.Amount < newQty)
            throw new Exception($"Недостаточно товара на складе. Доступно: {sku.Amount}");

        if (existing is not null)
        {
            existing.Quantity = newQty;
        }
        else
        {
            _db.Carts.Add(new Cart { UserId = userId, SkuId = skuId, Quantity = quantity });
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(Guid userId, Guid skuId, int quantity)
    {
        var sku = await _db.Skus.FindAsync(skuId)
            ?? throw new Exception("SKU не найден");

        if (sku.Amount < quantity)
            throw new Exception($"Недостаточно товара на складе. Доступно: {sku.Amount}");

        var item = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.SkuId == skuId)
            ?? throw new Exception("Позиция не найдена в корзине");

        item.Quantity = quantity;
        await _db.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(Guid userId, Guid skuId)
    {
        var item = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.SkuId == skuId);
        if (item is not null)
        {
            _db.Carts.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

    public async Task MergeAsync(Guid userId, List<CartMergeItem> localItems)
    {
        foreach (var local in localItems)
        {
            var sku = await _db.Skus.FindAsync(local.SkuId);
            if (sku is null) continue;

            var existing = await _db.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.SkuId == local.SkuId);

            var merged = existing is not null
                ? Math.Max(existing.Quantity, local.Quantity)
                : local.Quantity;

            merged = Math.Min(merged, (int)sku.Amount); // не больше остатка

            if (existing is not null)
                existing.Quantity = merged;
            else
                _db.Carts.Add(new Cart { UserId = userId, SkuId = local.SkuId, Quantity = merged });
        }

        await _db.SaveChangesAsync();
    }

    public async Task ClearAsync(Guid userId)
    {
        var items = _db.Carts.Where(c => c.UserId == userId);
        _db.Carts.RemoveRange(items);
        await _db.SaveChangesAsync();
    }
}
