using PhoneStore.Data;

namespace PhoneStore.Models.ViewModels;

public class OrderDetailViewModel
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public EOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }

    public DeliveryDetailViewModel? Delivery { get; set; }
    public List<OrderItemDetailViewModel> Items { get; set; } = new();
}

public class DeliveryDetailViewModel
{
    public EDeliveryType Type { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Comment { get; set; }
}

public class OrderItemDetailViewModel
{
    public Guid SkuId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal { get; set; }
}
