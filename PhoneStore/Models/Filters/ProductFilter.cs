using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class ProductFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem Title { get; set; } = new FilterItem();
        public FilterItem BrandId { get; set; } = new FilterItem();
    }
}
