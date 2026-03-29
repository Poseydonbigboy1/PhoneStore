using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<Sku> CreateSkus()
        {
            var products = CreateProducts();

            return new List<Sku>()
            {
                new Sku()
                {
                    ProductId = products[0].Id,
                    Price = 1099.99,
                    Amount = 10,
                    Discount = 0.1
                },
                new Sku()
                {
                    ProductId = products[0].Id,
                    Price = 2099.99,
                    Amount = 10,
                    Discount = 0.1
                }
            };
        }
    }
}