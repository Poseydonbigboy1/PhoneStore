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
        public ProductSortBy SortBy { get; set; } = ProductSortBy.None;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public List<ProductFilterValue> FilterValues { get; set; } = new List<ProductFilterValue>();
    }

    public class ProductFilterValue
    {
        public string? ComponentTitle { get; set; }
        public string? Value { get; set; }
        public string? MatchMode { get; set; }
    }

    public enum ProductSortBy
    {
        None = 0,
        Price = 1,
        Popularity = 2
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}