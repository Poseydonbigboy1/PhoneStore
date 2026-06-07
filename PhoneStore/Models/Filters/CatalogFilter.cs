using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Models.Filters
{
    public class CatalogFilter
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public CatalogSortBy SortBy { get; set; } = CatalogSortBy.None;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public string? Search { get; set; }
        public List<CatalogFilterValue> FilterValues { get; set; } = new List<CatalogFilterValue>();
    }

    public class CatalogFilterValue
    {
        public string? ComponentTitle { get; set; }
        public string? Value { get; set; }
        public string? MatchMode { get; set; }
    }

    public enum CatalogSortBy
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