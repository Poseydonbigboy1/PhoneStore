using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class SkuFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem ProductId { get; set; } = new FilterItem();
        public FilterItem Price { get; set; } = new FilterItem();
        public FilterItem Amount { get; set; } = new FilterItem();
        public FilterItem Discount { get; set; } = new FilterItem();
    }
}
