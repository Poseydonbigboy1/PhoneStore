using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<Sku> CreateSkus(List<Product> products)
        {
            var productsDict = products.ToDictionary(p => p.Title, p => p.Id);

            return new List<Sku>()
            {
                // SKU для iPhone 15 Pro
                new Sku()
                {
                    ProductId = productsDict["iPhone 15 Pro"],
                    Price = 1199.00,
                    Amount = 20,
                    Discount = 0
                },
                new Sku()
                {
                    ProductId = productsDict["iPhone 15 Pro"],
                    Price = 1399.00,
                    Amount = 15,
                    Discount = 0
                },

                // SKU для Samsung Galaxy S25 Ultra
                new Sku()
                {
                    ProductId = productsDict["Samsung Galaxy S25 Ultra"],
                    Price = 1299.00,
                    Amount = 25,
                    Discount = 0.05 // Скидка 5%
                },
                new Sku()
                {
                    ProductId = productsDict["Samsung Galaxy S25 Ultra"],
                    Price = 1499.00,
                    Amount = 10,
                    Discount = 0.05 // Скидка 5%
                }
            };
        }
    }
}