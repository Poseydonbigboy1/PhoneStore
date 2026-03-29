using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneStore.Data;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Внешний ключ для пользователя
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public EOrderStatus Status { get; set; } = EOrderStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    public string ShippingAddress { get; set; } = string.Empty;
    
    // Список товаров в этом заказе
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
