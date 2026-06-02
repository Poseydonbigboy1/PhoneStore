using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class ProductComponentFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem SkuId { get; set; } = new FilterItem();
        public FilterItem ComponentId { get; set; } = new FilterItem();
        public FilterItem ValueJson { get; set; } = new FilterItem();
        public FilterItem Filtering { get; set; } = new FilterItem();
    }
}
