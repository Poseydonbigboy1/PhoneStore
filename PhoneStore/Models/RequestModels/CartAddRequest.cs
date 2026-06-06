namespace PhoneStore.Models.RequestModels;

public class CartAddRequest
{
    public Guid SkuId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class CartUpdateRequest
{
    public int Quantity { get; set; }
}
