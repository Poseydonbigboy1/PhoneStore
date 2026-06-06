using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class UserFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem Login { get; set; } = new FilterItem();
        public FilterItem Name { get; set; } = new FilterItem();
        public FilterItem Roles { get; set; } = new FilterItem();
    }
}
