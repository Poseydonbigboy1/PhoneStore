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
            var baseQuery = _db.ProductComponents
                .Include(i => i.Component)
                .Include(i => i.Sku)
                    .ThenInclude(i => i.Product);
            
            var skusFiltered = new HashSet<Guid>();
            if (filter.FilterValues != null && filter.FilterValues.Any())
            {
                foreach (var f in filter.FilterValues.Where(x => !string.IsNullOrWhiteSpace(x.ComponentTitle)))
                {
                    var componentTitle = f.ComponentTitle.Trim();
                    var value = f.Value?.Trim();
                    var mode = (f.MatchMode ?? "equals").ToLowerInvariant();

                    var partialSkuIds = baseQuery
                        .Where(pc => pc.Component.Title == componentTitle)
                        .AsEnumerable()
                        .Where(pc => IsComponentValueMatch(pc, value, mode))
                        .Select(pc => pc.SkuId)
                        .Distinct()
                        .ToList();

                    if (!skusFiltered.Any())
                    {
                        foreach (var id in partialSkuIds)
                            skusFiltered.Add(id);
                    }
                    else
                    {
                        skusFiltered.IntersectWith(partialSkuIds);
                    }
                }
            }

            IQueryable<ProductComponent> filteredQuery = baseQuery;
            if (skusFiltered.Any())
            {
                filteredQuery = filteredQuery.Where(pc => skusFiltered.Contains(pc.SkuId));
            }

            var products = filteredQuery
                .AsEnumerable()
                .GroupBy(pc => pc.SkuId)
                .Select(g => new PoductViewModel
                {
                    Title = g.First().Sku.Product.Title,
                    Price = g.First().Sku.Price,
                    Discount = g.First().Sku.Discount,
                    Components = g.Select(pc => new ComponentViewModel
                    {
                        Title = pc.Component.Title,
                        Description = pc.Component.Description,
                        DataType = pc.Component.DataType
                    }).ToList()
                })
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToList();

            return products;
        }

        private static bool IsComponentValueMatch(ProductComponent pc, string? value, string mode)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            var actual = pc.Value?.Value?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(actual))
                return false;

            return mode switch
            {
                "contains" => actual.Contains(value, StringComparison.OrdinalIgnoreCase),
                "startsWith" => actual.StartsWith(value, StringComparison.OrdinalIgnoreCase),
                "endsWith" => actual.EndsWith(value, StringComparison.OrdinalIgnoreCase),
                _ => string.Equals(actual, value, StringComparison.OrdinalIgnoreCase),
            };
        }
    }
}