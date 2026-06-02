using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class ComponentCategoryFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem Title { get; set; } = new FilterItem();
    }
}
