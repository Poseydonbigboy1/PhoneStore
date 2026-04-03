using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<Brand> CreateBrands()
        {
            return new List<Brand>
            {
                new Brand { Id = Guid.NewGuid(), Title = "Apple" },
                new Brand { Id = Guid.NewGuid(), Title = "Samsung" }
            };
        }
    }
}
