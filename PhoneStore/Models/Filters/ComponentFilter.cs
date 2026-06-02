using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class ComponentFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem Title { get; set; } = new FilterItem();
        public FilterItem ComponentCategoryId { get; set; } = new FilterItem();
        public FilterItem DataType { get; set; } = new FilterItem();
        public FilterItem CategoryType { get; set; } = new FilterItem();
    }
}
