using System.Collections.Generic;

namespace PhoneStore.Models
{
    public class FilterResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Total { get; set; } = 0;
    }
}
