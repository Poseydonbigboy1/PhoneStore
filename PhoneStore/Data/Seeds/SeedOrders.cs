namespace PhoneStore.Data.Seeds;

public static partial class SeedData
{
    // Доабвялет пару заказов в БД
    public static List<Order> CreateOrders(List<User> users, List<Sku> skus)
    {
        var orders = new List<Order>();
        var random = new Random();

        // Создаем заказы для первого пользователя
        var user1 = users.First(u => u.Login == "test");
        var user1Skus = skus.Take(2).ToList(); // Берем первые два SKU для примера

        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            OrderDate = DateTime.UtcNow.AddDays(-10),
            Status = EOrderStatus.Delivered,
            ShippingAddress = "123 Main St, Anytown, USA",
            OrderItems = new List<OrderItem>()
        };

        foreach (var sku in user1Skus)
        {
            var quantity = random.Next(1, 3);
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order1.Id,
                SkuId = sku.Id,
                Quantity = quantity,
                Price = (decimal)sku.Price // Фиксируем цену на момент покупки
            };
            order1.OrderItems.Add(orderItem);
        }
        order1.TotalAmount = order1.OrderItems.Sum(oi => oi.Price * oi.Quantity);
        orders.Add(order1);

        // Создаем заказы для второго пользователя
        var user2 = users.First(u => u.Login == "test2");
        var user2Skus = skus.Skip(2).Take(1).ToList(); // Берем следующий SKU

        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            Status = EOrderStatus.Shipped,
            ShippingAddress = "456 Oak Ave, Someplace, USA",
            OrderItems = new List<OrderItem>()
        };
        
        var skuForOrder2 = user2Skus.First();
        var quantity2 = 1;
        var orderItem2 = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = order2.Id,
            SkuId = skuForOrder2.Id,
            Quantity = quantity2,
            Price = (decimal)skuForOrder2.Price
        };
        order2.OrderItems.Add(orderItem2);
        order2.TotalAmount = order2.OrderItems.Sum(oi => oi.Price * oi.Quantity);
        orders.Add(order2);

        return orders;
    }
}
