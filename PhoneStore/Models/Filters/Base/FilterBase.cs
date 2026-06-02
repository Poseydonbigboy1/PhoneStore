using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Models.Filters.Base
{
    public class FilterBase
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public string? SortBy { get; set; } = null;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    }

    public enum ESortDirection
    {
        Ascending = 0,
        Descending = 1
    }

}