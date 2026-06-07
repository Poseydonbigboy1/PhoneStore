namespace PhoneStore.Data;

public class Wishlist : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid SkuId { get; set; }
    public Sku? Sku { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
