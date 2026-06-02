using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class OrderItemFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem OrderId { get; set; } = new FilterItem();
        public FilterItem SkuId { get; set; } = new FilterItem();
        public FilterItem Quantity { get; set; } = new FilterItem();
        public FilterItem Price { get; set; } = new FilterItem();
    }
}
