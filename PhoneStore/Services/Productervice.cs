using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;

namespace PhoneStore.Services
{
    public class ProductService
    {
        private readonly ApplicationContext _db;
        public ProductService(ApplicationContext db)
        {
            _db = db;
        }


        public IEnumerable<PoductViewModel> GetProductsByFilter(ProductFilter filter)
        {
            var products = _db.ProductComponents
                .Include(i => i.Component)
                .Include(i => i.Sku)
                    .ThenInclude(i => i.Product)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(pc => new PoductViewModel
                {
                    Title = pc.Sku.Product.Title,
                    Price = pc.Value.Price,
                    Discount = pc.Sku.Discount,
                    Components = new List<ComponentViewModel>
                    {
                        new ComponentViewModel
                        {
                            Title = pc.Component.Title,
                            Description = pc.Component.Description,
                            DataType = pc.Component.DataType
                        }
                    }
                })
                .ToList();

            return products;
        }
    }
}