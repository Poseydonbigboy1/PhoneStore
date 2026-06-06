using PhoneStore.Data;

namespace PhoneStore.Models.ViewModels;

public class OrderSummaryViewModel
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public EOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public EDeliveryType? DeliveryType { get; set; }
}
