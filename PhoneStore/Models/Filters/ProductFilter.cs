using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Models.Filters
{
    public class ProductFilter
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public List<ProductFilterValue> FilterValues { get; set; } = [];
    }

    public class ProductFilterValue
    {
        public string ComponentTitle { get; set; }
        public string Value { get; set; }
        public string MatchMode { get; set; }
    }
}