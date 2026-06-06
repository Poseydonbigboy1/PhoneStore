namespace PhoneStore.Models.ViewModels;

public class CartItemViewModel
{
    public Guid SkuId { get; set; }
    public int Quantity { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public int Amount { get; set; }   // остаток на складе
}
