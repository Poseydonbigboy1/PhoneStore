using System;
using System.Collections.Generic;

namespace PhoneStore.Data
{
    public class Brand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
