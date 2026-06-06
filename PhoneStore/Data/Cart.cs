namespace PhoneStore.Data;

public class Cart : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid SkuId { get; set; }
    public Sku? Sku { get; set; }
    public int Quantity { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
