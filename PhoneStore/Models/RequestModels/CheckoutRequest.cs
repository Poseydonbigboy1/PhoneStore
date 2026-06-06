using PhoneStore.Data;

namespace PhoneStore.Models.RequestModels;

public class CheckoutRequest
{
    public DeliveryInfo Delivery { get; set; } = new();
}

public class DeliveryInfo
{
    public EDeliveryType Type { get; set; } = EDeliveryType.Courier;
    public string RecipientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Comment { get; set; }
}
