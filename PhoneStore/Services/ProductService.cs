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

        public IEnumerable<object> GetFilters()
        {

            var filterBrands = _db.Products
                .Where(p => p.Brand != null)
                .Select(p => new
                {
                    group_title = "Брэнд",
                    data_type = 0,
                    filter_value = p.Brand.Title
                })
                .Distinct()
                .ToList();

            var minPrice = _db.Skus.Min(s => (decimal?)s.Price) ?? 0;
            var maxPrice = _db.Skus.Max(s => (decimal?)s.Price) ?? 0;

            var filterPrice = new List<object>{
                new
                {
                    group_title = "Цена",
                    data_type = 1,
                    filter_value = minPrice
                },
                new
                {
                    group_title = "Цена",
                    data_type = 1,
                    filter_value = maxPrice
                }
            };

            var filters = _db.Components
                .Join(
                    _db.ProductComponents,
                    c => c.Id,
                    pc => pc.ComponentId,
                    (c, pc) => new { c, pc }
                )
                .Where(joined => joined.pc.Filtering == true)
                .Select(joined => new
                {
                    group_title = joined.c.Title,
                    data_type = joined.c.DataType,
                    filter_value = joined.pc.ValueJson
                })
                .Distinct()
                .OrderBy(x => x.group_title)
                .ToList();
            return [.. filterBrands, .. filterPrice, .. filters];
        }


        public ProductsResult GetProductsByFilter(ProductFilter filter)
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

            var popularityBySku = _db.OrderItems
                .GroupBy(oi => oi.SkuId)
                .Select(g => new
                {
                    SkuId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .ToDictionary(x => x.SkuId, x => x.TotalQuantity);

            var productsWithPopularity = filteredQuery
                .AsEnumerable()
                .GroupBy(pc => pc.SkuId)
                .Select(g => new
                {
                    SkuId = g.Key,
                    Product = new PoductViewModel
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
                    },
                    Popularity = popularityBySku.TryGetValue(g.Key, out var value) ? value : 0
                })
                .ToList();

            var totalCount = productsWithPopularity.Count;

            productsWithPopularity = filter.SortBy switch
            {
                ProductSortBy.Price when filter.SortDirection == SortDirection.Descending =>
                    productsWithPopularity.OrderByDescending(x => x.Product.Price),
                ProductSortBy.Price =>
                    productsWithPopularity.OrderBy(x => x.Product.Price),
                ProductSortBy.Popularity when filter.SortDirection == SortDirection.Descending =>
                    productsWithPopularity.OrderByDescending(x => x.Popularity),
                ProductSortBy.Popularity =>
                    productsWithPopularity.OrderBy(x => x.Popularity),
                _ => productsWithPopularity.OrderBy(x => x.Product.Title),
            };

            var products = productsWithPopularity
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(x => x.Product)
                .ToList();

            return new ProductsResult
            {
                Count = totalCount,
                Products = products
            };
        }

        private static bool IsComponentValueMatch(ProductComponent pc, string? value, string mode)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            var actual = pc.Value?.ToString() ?? string.Empty;
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