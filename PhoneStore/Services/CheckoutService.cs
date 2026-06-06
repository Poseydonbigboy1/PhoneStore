using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models.RequestModels;
using PhoneStore.Models.ViewModels;

namespace PhoneStore.Services;

public class CheckoutService
{
    private readonly ApplicationContext _db;

    public CheckoutService(ApplicationContext db)
    {
        _db = db;
    }

    public async Task<Guid> CheckoutAsync(Guid userId, CheckoutRequest request)
    {
        // Загружаем корзину
        var cartItems = await _db.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Sku)
            .ToListAsync();

        if (!cartItems.Any())
            throw new Exception("Корзина пуста");

        // Проверяем остатки и считаем итог
        decimal total = 0;
        foreach (var item in cartItems)
        {
            var sku = item.Sku!;
            if (sku.Amount < item.Quantity)
                throw new Exception($"Товар «{sku.ProductId}» недоступен в нужном количестве. Доступно: {sku.Amount}");

            var finalPrice = Math.Round((decimal)sku.Price * (1m - (decimal)sku.Discount / 100m), 2);
            total += finalPrice * item.Quantity;
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            // Создаём заказ
            var order = new Order
            {
                UserId      = userId,
                OrderDate   = DateTime.UtcNow,
                Status      = EOrderStatus.Pending,
                TotalAmount = total,
                ShippingAddress = request.Delivery.Address ?? string.Empty,
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Создаём позиции и списываем остатки
            foreach (var item in cartItems)
            {
                var sku = item.Sku!;
                var finalPrice = Math.Round((decimal)sku.Price * (1m - (decimal)sku.Discount / 100m), 2);

                _db.OrderItems.Add(new OrderItem
                {
                    OrderId  = order.Id,
                    SkuId    = item.SkuId,
                    Quantity = item.Quantity,
                    Price    = finalPrice,
                });

                sku.Amount -= item.Quantity;
            }

            // Создаём доставку
            _db.Deliveries.Add(new Delivery
            {
                OrderId       = order.Id,
                Type          = request.Delivery.Type,
                RecipientName = request.Delivery.RecipientName,
                Phone         = request.Delivery.Phone,
                Address       = request.Delivery.Address,
                Comment       = request.Delivery.Comment,
            });

            // Очищаем корзину
            _db.Carts.RemoveRange(cartItems);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return order.Id;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderSummaryViewModel>> GetMyOrdersAsync(Guid userId)
    {
        return await _db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderSummaryViewModel
            {
                Id           = o.Id,
                OrderDate    = o.OrderDate,
                Status       = o.Status,
                TotalAmount  = o.TotalAmount,
                ItemCount    = o.OrderItems.Count,
                DeliveryType = o.Delivery != null ? o.Delivery.Type : (EDeliveryType?)null,
            })
            .ToListAsync();
    }

    public async Task<OrderDetailViewModel?> GetMyOrderDetailAsync(Guid userId, Guid orderId)
    {
        var order = await _db.Orders
            .Include(o => o.Delivery)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Sku).ThenInclude(s => s!.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null) return null;

        return new OrderDetailViewModel
        {
            Id          = order.Id,
            OrderDate   = order.OrderDate,
            Status      = order.Status,
            TotalAmount = order.TotalAmount,
            Delivery    = order.Delivery is null ? null : new DeliveryDetailViewModel
            {
                Type          = order.Delivery.Type,
                RecipientName = order.Delivery.RecipientName,
                Phone         = order.Delivery.Phone,
                Address       = order.Delivery.Address,
                Comment       = order.Delivery.Comment,
            },
            Items = order.OrderItems.Select(oi => new OrderItemDetailViewModel
            {
                SkuId        = oi.SkuId,
                ProductTitle = oi.Sku?.Product?.Title ?? string.Empty,
                Quantity     = oi.Quantity,
                Price        = oi.Price,
                LineTotal    = oi.Price * oi.Quantity,
            }).ToList(),
        };
    }
}
