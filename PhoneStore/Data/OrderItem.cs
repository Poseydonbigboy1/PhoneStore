using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneStore.Data;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Внешний ключ для заказа
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    // Внешний ключ для купленного товара (вариации)
    public Guid SkuId { get; set; }
    public Sku Sku { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    // Цена на момент покупки (ценовой снимок)
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}
