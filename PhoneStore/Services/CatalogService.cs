using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Models.Filters;

#nullable disable

namespace PhoneStore.Services
{
    public class CatalogService
    {
        private readonly ApplicationContext _db;
        public CatalogService(ApplicationContext db)
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
                .Where(joined => joined.pc.Filtering == true && joined.c.DataType != EDataType.IMAGE)
                .Select(joined => new
                {
                    group_title = joined.c.Title,
                    data_type = joined.c.DataType,
                    filter_value = joined.pc.ValueJson
                })
                .Distinct()
                .OrderBy(x => x.group_title)
                .ToList();
                var combined = new List<object>();
                combined.AddRange(filterBrands.Cast<object>());
                combined.AddRange(filterPrice);
                combined.AddRange(filters.Cast<object>());
                return combined;
        }


        public CatalogResult GetProductsByFilter(CatalogFilter filter)
        {
            IQueryable<ProductComponent> baseQuery = _db.ProductComponents
                .Include(i => i.Component)
                .Include(i => i.Sku)
                    .ThenInclude(i => i.Product!);

            // Текстовый поиск по названию товара
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();
                baseQuery = baseQuery.Where(pc => pc.Sku!.Product!.Title.ToLower().Contains(search));
            }

            var skusFiltered = new HashSet<Guid>();
            if (filter.FilterValues != null && filter.FilterValues.Any())
            {
                var groups = filter.FilterValues
                    .Where(x => !string.IsNullOrWhiteSpace(x.ComponentTitle))
                    .GroupBy(x => x.ComponentTitle!.Trim(), StringComparer.OrdinalIgnoreCase);

                foreach (var group in groups)
                {
                    var componentTitle = group.Key;

                    List<Guid> groupSkuIds = new List<Guid>();

                    if (string.Equals(componentTitle, "цена", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(componentTitle, "price", StringComparison.OrdinalIgnoreCase))
                    {
                        double? min = null;
                        double? max = null;

                        foreach (var f in group)
                        {
                            var mode = (f.MatchMode ?? "equals").ToLowerInvariant();
                            var val = f.Value?.Trim();
                            if (string.IsNullOrWhiteSpace(val))
                                continue;

                            if (mode == "between")
                            {
                                var parts = val.Split('-', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2 && double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var a) && double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var b))
                                {
                                    var lo = Math.Min(a, b);
                                    var hi = Math.Max(a, b);
                                    min = min.HasValue ? Math.Max(min.Value, lo) : lo;
                                    max = max.HasValue ? Math.Min(max.Value, hi) : hi;
                                }
                            }
                            else if (mode == "gte")
                            {
                                if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                                    min = min.HasValue ? Math.Max(min.Value, v) : v;
                            }
                            else if (mode == "lte")
                            {
                                if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                                    max = max.HasValue ? Math.Min(max.Value, v) : v;
                            }
                            else if (mode == "equals")
                            {
                                if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                                {
                                    min = min.HasValue ? Math.Max(min.Value, v) : v;
                                    max = max.HasValue ? Math.Min(max.Value, v) : v;
                                }
                            }
                        }

                        var skuQuery = _db.Skus.Include(s => s.Product).ThenInclude(p => p.Brand).AsQueryable();
                        if (min.HasValue)
                            skuQuery = skuQuery.Where(s => s.Price >= min.Value);
                        if (max.HasValue)
                            skuQuery = skuQuery.Where(s => s.Price <= max.Value);

                        groupSkuIds = skuQuery.Select(s => s.Id).Distinct().ToList();
                    }
                    else if (string.Equals(componentTitle, "брэнд", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(componentTitle, "brand", StringComparison.OrdinalIgnoreCase))
                    {
                        var union = new HashSet<Guid>();
                        foreach (var f in group)
                        {
                            var mode = (f.MatchMode ?? "equals").ToLowerInvariant();
                            var val = f.Value?.Trim();
                            var matched = _db.Skus
                                .Include(s => s.Product)
                                    .ThenInclude(p => p.Brand)
                                .AsEnumerable()
                                .Where(s => s.Product != null && s.Product.Brand != null && IsStringMatch(s.Product.Brand.Title, val, mode))
                                .Select(s => s.Id)
                                .Distinct()
                                .ToList();
                            foreach (var id in matched) union.Add(id);
                        }
                        groupSkuIds = union.ToList();
                    }
                    else
                    {
                        var union = new HashSet<Guid>();
                        foreach (var f in group)
                        {
                            var mode = (f.MatchMode ?? "equals").ToLowerInvariant();
                            var val = f.Value?.Trim();
                            var matched = baseQuery
                                .Where(pc => pc.Component!.Title == componentTitle)
                                .AsEnumerable()
                                .Where(pc => IsComponentValueMatch(pc, val, mode))
                                .Select(pc => pc.SkuId)
                                .Distinct()
                                .ToList();
                            foreach (var id in matched) union.Add(id);
                        }
                        groupSkuIds = union.ToList();
                    }

                    if (!skusFiltered.Any())
                    {
                        foreach (var id in groupSkuIds)
                            skusFiltered.Add(id);
                    }
                    else
                    {
                        skusFiltered.IntersectWith(groupSkuIds);
                    }
                }
            }

            IQueryable<ProductComponent> filteredQuery = baseQuery;
            if (filter.FilterValues != null && filter.FilterValues.Any())
            {
                if (skusFiltered.Any())
                {
                    filteredQuery = filteredQuery.Where(pc => skusFiltered.Contains(pc.SkuId));
                }
                else
                {
                    // Если были фильтры, но результат пуст (AND логика вернула пусто), вернём пустой результат
                    filteredQuery = filteredQuery.Where(pc => false);
                }
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
                        ProductId = g.First().Sku!.Product!.Id,
                        SkuId = g.Key,
                        Title = g.First().Sku!.Product!.Title,
                        Price = g.First().Sku!.Price,
                        Discount = g.First().Sku!.Discount,
                        Images = g.Where(pc => pc.Component!.DataType == EDataType.IMAGE)
                                  .Select(pc => pc.Value?.ToString() ?? string.Empty)
                                  .Where(s => !string.IsNullOrEmpty(s))
                                  .ToList(),
                        Components = g.Where(pc => pc.Component!.DataType != EDataType.IMAGE)
                                      .Select(pc => new ComponentViewModel
                                      {
                                          Title = pc.Component!.Title,
                                          Description = pc.Component!.Description,
                                          DataType = pc.Component.DataType
                                      }).ToList()
                    },
                    Popularity = popularityBySku.TryGetValue(g.Key, out var value) ? value : 0
                });

            var totalCount = productsWithPopularity.Count();

            productsWithPopularity = filter.SortBy switch
            {
                CatalogSortBy.Price when filter.SortDirection == SortDirection.Descending =>
                    productsWithPopularity.OrderByDescending(x => x.Product.Price),
                CatalogSortBy.Price =>
                    productsWithPopularity.OrderBy(x => x.Product.Price),
                CatalogSortBy.Popularity when filter.SortDirection == SortDirection.Descending =>
                    productsWithPopularity.OrderByDescending(x => x.Popularity),
                CatalogSortBy.Popularity =>
                    productsWithPopularity.OrderBy(x => x.Popularity),
                _ => productsWithPopularity.OrderBy(x => x.Product.Title),
            };

            var products = productsWithPopularity
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(x => x.Product)
                .ToList();

            return new CatalogResult
            {
                Count = totalCount,
                Products = products
            };
        }

        private static bool IsComponentValueMatch(ProductComponent pc, string value, string mode)
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

        private static string GetComponentValueAsString(ProductComponent pc)
        {
            if (pc.Value == null)
                return string.Empty;

            return pc.Value switch
            {
                string s => s,
                bool b => b.ToString(),
                double d => d.ToString(CultureInfo.InvariantCulture),
                float f => f.ToString(CultureInfo.InvariantCulture),
                int i => i.ToString(CultureInfo.InvariantCulture),
                long l => l.ToString(CultureInfo.InvariantCulture),
                decimal m => m.ToString(CultureInfo.InvariantCulture),
                _ => pc.Value.ToString() ?? string.Empty,
            };
        }

        private static bool IsStringMatch(string actual, string value, string mode)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            actual = actual ?? string.Empty;
            if (string.IsNullOrEmpty(actual))
                return false;

            return mode switch
            {
                "contains" => actual.Contains(value, StringComparison.OrdinalIgnoreCase),
                "startswith" => actual.StartsWith(value, StringComparison.OrdinalIgnoreCase),
                "endswith" => actual.EndsWith(value, StringComparison.OrdinalIgnoreCase),
                _ => string.Equals(actual, value, StringComparison.OrdinalIgnoreCase),
            };
        }

        /// <summary>Получить полную информацию о конкретном товаре (для карточки товара)</summary>
        public ProductCardViewModel? GetProductById(Guid productId)
        {
            var product = _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Skus)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
                return null;

            // Загружаем все компоненты для товара
            var skuIds = product.Skus.Select(s => s.Id).ToList();
            var allProductComponents = _db.ProductComponents
                .Include(pc => pc.Component)
                .Where(pc => skuIds.Contains(pc.SkuId))
                .ToList();

            // Изображения — IMAGE-компоненты (берём из первого SKU, они одинаковы для продукта)
            var images = allProductComponents
                .Where(pc => pc.Component!.DataType == EDataType.IMAGE)
                .Select(pc => pc.Value?.ToString() ?? string.Empty)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToList();

            // Общие компоненты: присутствуют во всех SKU И значение одинаково, исключая IMAGE
            var commonComponentIds = allProductComponents
                .Where(pc => pc.Component!.DataType != EDataType.IMAGE)
                .GroupBy(pc => pc.ComponentId)
                .Where(g =>
                    g.Select(pc => pc.SkuId).Distinct().Count() == product.Skus.Count
                    && g.Select(pc => GetComponentValueAsString(pc)).Distinct().Count() == 1)
                .Select(g => g.Key)
                .ToHashSet();

            // Все IMAGE-компонент ID, чтобы исключать их из SKU-специфичных тоже
            var imageComponentIds = allProductComponents
                .Where(pc => pc.Component!.DataType == EDataType.IMAGE)
                .Select(pc => pc.ComponentId)
                .ToHashSet();

            var commonComponents = allProductComponents
                .Where(pc => commonComponentIds.Contains(pc.ComponentId))
                .GroupBy(pc => pc.ComponentId)
                .Select(g => g.First())
                .Select(pc => new ComponentViewModel
                {
                    Title = pc.Component!.Title,
                    Description = pc.Component.Description,
                    DataType = pc.Component.DataType,
                    Value = GetComponentValueAsString(pc)
                })
                .ToList();

            // Строим SKU с их специфичными компонентами
            var skuModels = product.Skus.Select(s =>
            {
                var skuComponents = allProductComponents
                    .Where(pc => pc.SkuId == s.Id && !commonComponentIds.Contains(pc.ComponentId) && !imageComponentIds.Contains(pc.ComponentId))
                    .Select(pc => new ComponentViewModel
                    {
                        Title = pc.Component!.Title,
                        Description = pc.Component.Description,
                        DataType = pc.Component.DataType,
                        Value = GetComponentValueAsString(pc)
                    })
                    .ToList();

                return new SkuCardViewModel
                {
                    SkuId = s.Id,
                    Price = s.Price,
                    Discount = s.Discount,
                    Amount = s.Amount,
                    SkuSpecificComponents = skuComponents
                };
            }).ToList();

            var mainSku = skuModels
                .OrderByDescending(s => s.Amount > 0)
                .ThenBy(s => s.Price)
                .FirstOrDefault() ?? new SkuCardViewModel();

            var additionalSkus = skuModels
                .Where(s => s.SkuId != mainSku.SkuId)
                .ToList();

            return new ProductCardViewModel
            {
                ProductId = product.Id,
                Title = product.Title,
                Description = product.Description,
                BrandTitle = product.Brand?.Title ?? string.Empty,
                Images = images,
                MainSku = mainSku,
                AdditionalSkus = additionalSkus,
                CommonComponents = commonComponents
            };
        }

        /// <summary>Топ товаров по количеству продаж</summary>
        public List<PoductViewModel> GetPopular(int take = 10)
        {
            var topSkuIds = _db.OrderItems
                .GroupBy(oi => oi.SkuId)
                .Select(g => new { SkuId = g.Key, Count = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .Select(x => x.SkuId)
                .ToList();

            if (!topSkuIds.Any())
            {
                // If no orders, just return newest SKUs
                return _db.Skus
                    .Include(s => s.Product).ThenInclude(p => p!.Brand)
                    .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                    .Where(s => s.Amount > 0)
                    .OrderByDescending(s => s.Id)
                    .Take(take)
                    .ToList()
                    .Select(s => MapToViewModel(s))
                    .ToList();
            }

            return _db.Skus
                .Include(s => s.Product).ThenInclude(p => p!.Brand)
                .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                .Where(s => topSkuIds.Contains(s.Id))
                .ToList()
                .OrderBy(s => topSkuIds.IndexOf(s.Id))
                .Select(s => MapToViewModel(s))
                .ToList();
        }

        /// <summary>Товары со скидкой, сортировка по размеру скидки</summary>
        public List<PoductViewModel> GetDiscounted(int take = 10)
        {
            return _db.Skus
                .Include(s => s.Product).ThenInclude(p => p!.Brand)
                .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                .Where(s => s.Discount > 0 && s.Amount > 0)
                .OrderByDescending(s => s.Discount)
                .Take(take)
                .ToList()
                .Select(s => MapToViewModel(s))
                .ToList();
        }

        /// <summary>Загрузить несколько SKU по массиву Id</summary>
        public List<PoductViewModel> GetBatch(List<Guid> skuIds)
        {
            return _db.Skus
                .Include(s => s.Product).ThenInclude(p => p!.Brand)
                .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                .Where(s => skuIds.Contains(s.Id))
                .ToList()
                .Select(s => MapToViewModel(s))
                .ToList();
        }

        private PoductViewModel MapToViewModel(Data.Sku s)
        {
            var imageUrl = s.ProductComponents
                .FirstOrDefault(pc => pc.Component?.DataType == EDataType.IMAGE)
                ?.Value?.ToString();

            return new PoductViewModel
            {
                SkuId      = s.Id,
                ProductId  = s.Product!.Id,
                Title      = s.Product.Title,
                BrandTitle = s.Product.Brand?.Title ?? string.Empty,
                Price      = s.Price,
                Discount   = s.Discount,
                Amount     = (int)s.Amount,
                ImageUrl   = imageUrl,
            };
        }

        /// <summary>Похожие товары — из того же бренда, исключая текущий SKU</summary>
        public List<PoductViewModel> GetSimilar(Guid skuId, int take = 6)
        {
            var sku = _db.Skus.Include(s => s.Product).ThenInclude(p => p!.Brand)
                .FirstOrDefault(s => s.Id == skuId);
            if (sku?.Product?.Brand is null) return new();

            var brandId = sku.Product.BrandId;

            var similar = _db.Skus
                .Include(s => s.Product).ThenInclude(p => p!.Brand)
                .Include(s => s.ProductComponents).ThenInclude(pc => pc.Component)
                .Where(s => s.Id != skuId && s.Product!.BrandId == brandId)
                .OrderBy(s => s.Product!.Title)
                .Take(take)
                .ToList();

            return similar.Select(s => MapToViewModel(s)).ToList();
        }
    }
}