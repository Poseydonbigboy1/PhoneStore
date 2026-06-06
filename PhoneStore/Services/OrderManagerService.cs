using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;

namespace PhoneStore.Services;

public class OrderManagerService
{
    private readonly ApplicationContext _db;

    public OrderManagerService(ApplicationContext db)
    {
        _db = db;
    }

    // Матрица допустимых переходов
    private static readonly Dictionary<EOrderStatus, EOrderStatus[]> AllowedTransitions = new()
    {
        { EOrderStatus.Pending,   [EOrderStatus.Confirmed, EOrderStatus.Cancelled] },
        { EOrderStatus.Confirmed, [EOrderStatus.Shipped,   EOrderStatus.Cancelled] },
        { EOrderStatus.Shipped,   [EOrderStatus.Delivered] },
        { EOrderStatus.Delivered, [] },
        { EOrderStatus.Cancelled, [] },
    };

    public async Task ChangeStatusAsync(Guid orderId, EOrderStatus newStatus)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Sku)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new Exception("Заказ не найден");

        if (!AllowedTransitions.TryGetValue(order.Status, out var allowed) || !allowed.Contains(newStatus))
            throw new Exception($"Переход из {order.Status} в {newStatus} недопустим");

        // При отмене — возвращаем остатки
        if (newStatus == EOrderStatus.Cancelled)
        {
            foreach (var item in order.OrderItems)
            {
                if (item.Sku is not null)
                    item.Sku.Amount += item.Quantity;
            }
        }

        order.Status = newStatus;
        await _db.SaveChangesAsync();
    }

    public async Task<List<Order>> GetAllWithDetailsAsync()
    {
        return await _db.Orders
            .Include(o => o.Delivery)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Sku).ThenInclude(s => s!.Product)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}
