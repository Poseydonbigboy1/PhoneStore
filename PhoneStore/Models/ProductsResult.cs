using System.Collections.Generic;

namespace PhoneStore.Models
{
    public class ProductsResult
    {
        public int Count { get; set; }
        public IEnumerable<PoductViewModel> Products { get; set; } = new List<PoductViewModel>();
    }
}
