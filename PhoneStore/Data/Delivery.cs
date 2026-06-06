namespace PhoneStore.Data;

public class Delivery : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public EDeliveryType Type { get; set; } = EDeliveryType.Courier;
    public string RecipientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Comment { get; set; }
}

public enum EDeliveryType
{
    Courier = 0,   // Курьер до двери
    Pickup  = 1    // Самовывоз
}
