using PhoneStore.Models.Filters.Base;

namespace PhoneStore.Models.Filters
{
    public class OrderFilter : FilterBase
    {
        public FilterItem Id { get; set; } = new FilterItem();
        public FilterItem UserId { get; set; } = new FilterItem();
        public FilterItem Status { get; set; } = new FilterItem();
        public FilterItem ShippingAddress { get; set; } = new FilterItem();
        public FilterItem TotalAmount { get; set; } = new FilterItem();
    }
}
