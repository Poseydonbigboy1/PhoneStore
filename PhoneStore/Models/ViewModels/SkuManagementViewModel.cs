namespace PhoneStore.Models.ViewModels;

public class SkuManagementViewModel
{
    public Guid   Id           { get; set; }
    public Guid   ProductId    { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string BrandTitle   { get; set; } = string.Empty;
    public double Price        { get; set; }
    public double Discount     { get; set; }
    public double FinalPrice   { get; set; }
    public double Amount       { get; set; }
    public List<SkuComponentView> Components { get; set; } = new();
}

public class SkuComponentView
{
    public Guid   ProductComponentId { get; set; }
    public Guid   ComponentId        { get; set; }
    public string ComponentTitle     { get; set; } = string.Empty;
    public string CategoryTitle      { get; set; } = string.Empty;
    public string Value              { get; set; } = string.Empty;
}

public class SkuUpsertRequest
{
    public Guid?  Id         { get; set; }   // null → create
    public Guid   ProductId  { get; set; }
    public double Price      { get; set; }
    public double Discount   { get; set; }
    public double Amount     { get; set; }
    public List<SkuComponentUpsert> Components { get; set; } = new();
}

public class SkuComponentUpsert
{
    public Guid?  ProductComponentId { get; set; }  // null → new
    public Guid   ComponentId        { get; set; }
    public string Value              { get; set; } = string.Empty;
}
