using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PhoneStore.Data;
using PhoneStore.Models.Import;

namespace PhoneStore.Services
{
    /// <summary>
    /// Импортирует телефоны из статического датасета (Data/Import/phones-dataset.json):
    /// генерирует Product + SKU (память × цвет) + ProductComponent, конвертирует цены USD→RUB
    /// по фиксированному курсу из конфига. Новые Component/ComponentCategory создаёт на лету
    /// с дедупликацией по синонимам, чтобы не плодить похожие справочники.
    /// </summary>
    public class PhoneImportService
    {
        private readonly ApplicationContext _db;
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;

        public PhoneImportService(ApplicationContext db, IConfiguration config, IHostEnvironment env)
        {
            _db = db;
            _config = config;
            _env = env;
        }

        // ── Метаданные компонента (определение характеристики) ────────────────
        private record ComponentSpec(string Title, EDataType DataType, string CategoryTitle, ECategoryType CategoryType, bool Filtering);

        // Канонические характеристики. Названия совпадают с существующими в сидах,
        // поэтому при импорте они переиспользуются, а не создаются заново.
        private static readonly Dictionary<string, ComponentSpec> CanonicalSpecs = new()
        {
            ["Операционная система"]        = new("Операционная система", EDataType.STRING, "Основные", ECategoryType.MAIN, true),
            ["Цвет"]                        = new("Цвет", EDataType.STRING, "Основные", ECategoryType.CASE, true),
            ["Изображение"]                 = new("Изображение", EDataType.IMAGE, "Основные", ECategoryType.OTHER, false),

            ["Диагональ экрана"]            = new("Диагональ экрана", EDataType.DOUBLE, "Экран", ECategoryType.DISPLAY, false),
            ["Тип матрицы"]                 = new("Тип матрицы", EDataType.STRING, "Экран", ECategoryType.DISPLAY, false),
            ["Разрешение экрана"]           = new("Разрешение экрана", EDataType.STRING, "Экран", ECategoryType.DISPLAY, false),
            ["Частота обновления экрана"]   = new("Частота обновления экрана", EDataType.DOUBLE, "Экран", ECategoryType.DISPLAY, true),

            ["ОЗУ"]                         = new("ОЗУ", EDataType.DOUBLE, "Память и процессор", ECategoryType.MAMORY, true),
            ["Встроенная память"]           = new("Встроенная память", EDataType.DOUBLE, "Память и процессор", ECategoryType.MAMORY, true),
            ["Процессор"]                   = new("Процессор", EDataType.STRING, "Память и процессор", ECategoryType.PROCESSOR, true),

            ["Основная камера"]             = new("Основная камера", EDataType.DOUBLE, "Камера", ECategoryType.CAMERA, false),
            ["Фронтальная камера"]          = new("Фронтальная камера", EDataType.DOUBLE, "Камера", ECategoryType.CAMERA, false),

            ["NFC"]                         = new("NFC", EDataType.BOOLEAN, "Связь", ECategoryType.COMUNICATION, true),
            ["Поддержка 5G"]                = new("Поддержка 5G", EDataType.BOOLEAN, "Связь", ECategoryType.COMUNICATION, true),
            ["Bluetooth"]                   = new("Bluetooth", EDataType.STRING, "Связь", ECategoryType.COMUNICATION, false),

            ["Емкость аккумулятора"]        = new("Емкость аккумулятора", EDataType.DOUBLE, "Питание", ECategoryType.BATTERY, false),
            ["Беспроводная зарядка"]        = new("Беспроводная зарядка", EDataType.BOOLEAN, "Питание", ECategoryType.BATTERY, true),

            ["Материал корпуса"]            = new("Материал корпуса", EDataType.STRING, "Корпус", ECategoryType.CASE, true),
            ["Класс защиты"]                = new("Класс защиты", EDataType.STRING, "Корпус", ECategoryType.CASE, false),
            ["Вес"]                         = new("Вес", EDataType.DOUBLE, "Корпус", ECategoryType.CASE, false),
        };

        // Синонимы → каноническое имя. Подстраховка от появления похожих компонентов.
        private static readonly Dictionary<string, string> Synonyms = new(StringComparer.OrdinalIgnoreCase)
        {
            ["оперативная память"] = "ОЗУ", ["ram"] = "ОЗУ", ["озу"] = "ОЗУ",
            ["пзу"] = "Встроенная память", ["накопитель"] = "Встроенная память", ["storage"] = "Встроенная память", ["память"] = "Встроенная память",
            ["акб"] = "Емкость аккумулятора", ["батарея"] = "Емкость аккумулятора", ["аккумулятор"] = "Емкость аккумулятора",
            ["цвета"] = "Цвет", ["color"] = "Цвет",
            ["ос"] = "Операционная система", ["os"] = "Операционная система",
            ["масса"] = "Вес",
            ["защита"] = "Класс защиты", ["влагозащита"] = "Класс защиты", ["ip"] = "Класс защиты",
            ["материал"] = "Материал корпуса", ["корпус"] = "Материал корпуса",
            ["изображения"] = "Изображение", ["фото"] = "Изображение", ["image"] = "Изображение",
        };

        // Рабочие словари (existing + созданные за импорт)
        private Dictionary<string, Brand> _brands = new();
        private Dictionary<string, ComponentCategory> _categories = new();
        private Dictionary<string, Component> _components = new();
        private HashSet<string> _existingProducts = new();

        private readonly Random _rnd = new(12345); // фиксированный seed — воспроизводимость

        public ImportSummary Import()
        {
            var summary = new ImportSummary();

            var rate = _config.GetSection("Settings").GetValue<double?>("UsdToRubRate") ?? 95.0;
            summary.UsdToRubRate = rate;

            var models = LoadDataset();

            // Предзагрузка справочников для дедупликации
            _brands = _db.Brands.ToList()
                .GroupBy(b => Norm(b.Title)).ToDictionary(g => g.Key, g => g.First());
            _categories = _db.ComponentCategories.ToList()
                .GroupBy(c => Norm(c.Title)).ToDictionary(g => g.Key, g => g.First());
            _components = _db.Components.ToList()
                .GroupBy(c => Norm(c.Title)).ToDictionary(g => g.Key, g => g.First());
            _existingProducts = _db.Products.ToList()
                .Select(p => ProductKey(p.BrandId, p.Title)).ToHashSet();

            foreach (var model in models)
            {
                var brand = GetOrCreateBrand(model.Brand, summary);

                var key = ProductKey(brand.Id, model.Title);
                if (_existingProducts.Contains(key))
                {
                    summary.ProductsSkipped++;
                    continue;
                }
                _existingProducts.Add(key);

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    BrandId = brand.Id,
                };
                _db.Products.Add(product);
                summary.ProductsCreated++;

                // Генерация SKU: память × цвет
                foreach (var tier in model.Storage)
                {
                    foreach (var color in model.Colors)
                    {
                        var priceUsd = model.BaseUsd + tier.AddUsd;
                        var priceRub = Math.Round(priceUsd * rate / 10.0) * 10.0; // округление до 10 ₽

                        var sku = new Sku
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            Price = priceRub,
                            Discount = model.Discount,
                            Amount = NextStock(),
                        };
                        _db.Skus.Add(sku);
                        summary.SkusCreated++;

                        // Вариативные характеристики (отличаются между SKU)
                        AddComponent(sku.Id, "Встроенная память", tier.Gb, summary);
                        AddComponent(sku.Id, "ОЗУ", tier.RamGb, summary);
                        AddComponent(sku.Id, "Цвет", color, summary);

                        // Общие характеристики (одинаковы для всех SKU товара)
                        AddComponent(sku.Id, "Операционная система", model.Os, summary);
                        AddComponent(sku.Id, "Процессор", model.Chipset, summary);
                        AddComponent(sku.Id, "Диагональ экрана", model.ScreenSize, summary);
                        AddComponent(sku.Id, "Тип матрицы", model.ScreenType, summary);
                        AddComponent(sku.Id, "Разрешение экрана", model.Resolution, summary);
                        AddComponent(sku.Id, "Частота обновления экрана", model.RefreshRate, summary);
                        AddComponent(sku.Id, "Основная камера", model.MainCamera, summary);
                        AddComponent(sku.Id, "Фронтальная камера", model.FrontCamera, summary);
                        AddComponent(sku.Id, "Емкость аккумулятора", model.Battery, summary);
                        AddComponent(sku.Id, "NFC", model.Nfc, summary);
                        AddComponent(sku.Id, "Поддержка 5G", model.FiveG, summary);
                        AddComponent(sku.Id, "Bluetooth", model.Bluetooth, summary);
                        AddComponent(sku.Id, "Беспроводная зарядка", model.WirelessCharging, summary);
                        AddComponent(sku.Id, "Материал корпуса", model.Material, summary);
                        AddComponent(sku.Id, "Класс защиты", model.IpRating, summary);
                        AddComponent(sku.Id, "Вес", model.Weight, summary);

                        // Изображения. Если в датасете заданы реальные URL — используем их;
                        // иначе генерируем плейсхолдер, окрашенный под цвет этого SKU.
                        if (model.Images.Count > 0)
                        {
                            foreach (var url in model.Images)
                                AddComponent(sku.Id, "Изображение", url, summary);
                        }
                        else
                        {
                            AddComponent(sku.Id, "Изображение", BuildImageUrl(model.Brand, model.Title, color), summary);
                        }
                    }
                }
            }

            _db.SaveChanges();

            summary.Notes.Add($"Импортировано моделей (товаров): {summary.ProductsCreated}, пропущено существующих: {summary.ProductsSkipped}.");
            summary.Notes.Add($"Создано SKU: {summary.SkusCreated}. Курс USD→RUB: {rate}.");
            if (summary.SkusCreated < 500)
                summary.Notes.Add("ВНИМАНИЕ: создано меньше 500 SKU — расширьте датасет phones-dataset.json.");

            return summary;
        }

        // ── Датасет ───────────────────────────────────────────────────────────
        private List<PhoneModelDto> LoadDataset()
        {
            var path = Path.Combine(_env.ContentRootPath, "Data", "Import", "phones-dataset.json");
            if (!File.Exists(path))
                throw new FileNotFoundException($"Не найден датасет телефонов: {path}");

            var json = File.ReadAllText(path);
            var models = JsonSerializer.Deserialize<List<PhoneModelDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (models == null || models.Count == 0)
                throw new InvalidOperationException("Датасет телефонов пуст или не распарсился.");

            return models;
        }

        // ── GetOrCreate с дедупликацией ───────────────────────────────────────
        private Brand GetOrCreateBrand(string title, ImportSummary summary)
        {
            var norm = Norm(title);
            if (_brands.TryGetValue(norm, out var existing))
                return existing;

            var brand = new Brand { Id = Guid.NewGuid(), Title = title.Trim() };
            _db.Brands.Add(brand);
            _brands[norm] = brand;
            summary.BrandsCreated++;
            return brand;
        }

        private ComponentCategory GetOrCreateCategory(string title, ImportSummary summary)
        {
            var norm = Norm(title);
            if (_categories.TryGetValue(norm, out var existing))
                return existing;

            var category = new ComponentCategory { Id = Guid.NewGuid(), Title = title.Trim() };
            _db.ComponentCategories.Add(category);
            _categories[norm] = category;
            summary.CategoriesCreated++;
            return category;
        }

        private Component GetOrCreateComponent(string title, ImportSummary summary)
        {
            var canonical = ResolveCanonical(title);
            var norm = Norm(canonical);
            if (_components.TryGetValue(norm, out var existing))
                return existing;

            var spec = CanonicalSpecs.TryGetValue(canonical, out var s)
                ? s
                : new ComponentSpec(canonical, EDataType.STRING, "Прочее", ECategoryType.OTHER, false);

            var category = GetOrCreateCategory(spec.CategoryTitle, summary);

            var component = new Component
            {
                Id = Guid.NewGuid(),
                Title = spec.Title,
                DataType = spec.DataType,
                CategoryType = spec.CategoryType,
                ComponentCategoryId = category.Id,
            };
            _db.Components.Add(component);
            _components[norm] = component;
            summary.ComponentsCreated++;
            return component;
        }

        private void AddComponent(Guid skuId, string componentTitle, object value, ImportSummary summary)
        {
            var component = GetOrCreateComponent(componentTitle, summary);
            var spec = CanonicalSpecs.TryGetValue(ResolveCanonical(componentTitle), out var s) ? s : null;

            _db.ProductComponents.Add(new ProductComponent
            {
                Id = Guid.NewGuid(),
                SkuId = skuId,
                ComponentId = component.Id,
                Value = value,
                Filtering = spec?.Filtering ?? false,
            });
            summary.ProductComponentsCreated++;
        }

        // ── Утилиты ───────────────────────────────────────────────────────────
        private static string ResolveCanonical(string title)
        {
            var trimmed = (title ?? string.Empty).Trim();
            return Synonyms.TryGetValue(trimmed, out var canonical) ? canonical : trimmed;
        }

        private static string Norm(string value) => (value ?? string.Empty).Trim().ToLowerInvariant();

        private static string ProductKey(Guid brandId, string title) => $"{brandId}|{Norm(title)}";

        // Цвет варианта → hex фона плейсхолдера (по ключевым словам, RU + EN).
        private static readonly (string key, string hex)[] ColorMap =
        {
            ("titanium", "8d9eab"), ("титан", "8d9eab"),
            ("black", "1a1a1a"), ("чёрн", "1a1a1a"), ("черн", "1a1a1a"), ("graphite", "2b2b2b"), ("obsidian", "1a1a1a"), ("midnight", "1c1c2e"), ("onyx", "1a1a1a"), ("phantom black", "151515"),
            ("white", "ededed"), ("бел", "ededed"), ("porcelain", "f0ece4"), ("starlight", "f5f2e9"), ("cream", "efe7d8"), ("snow", "f4f4f4"), ("silver", "c8c8c8"), ("платин", "d6d6d6"), ("серебр", "c8c8c8"),
            ("gray", "808080"), ("grey", "808080"), ("сер", "808080"), ("gray", "808080"), ("storm", "9aa0a6"),
            ("blue", "1565c0"), ("син", "1565c0"), ("голуб", "4a90d9"), ("bay", "3b6ea5"), ("navy", "1b2a4a"), ("cobalt", "2a3f7a"), ("sierra blue", "8fb8d8"),
            ("green", "2e7d32"), ("зелен", "2e7d32"), ("зелён", "2e7d32"), ("emerald", "1f6e4a"), ("mint", "9fd8c4"), ("alpine", "3a5f4a"), ("khaki", "6b6b3a"), ("meadow", "3f7d4f"),
            ("red", "c62828"), ("красн", "c62828"), ("magenta", "b5316a"), ("burgundy", "6e1f2e"), ("coral", "e57373"),
            ("gold", "c9942a"), ("золот", "c9942a"), ("amber", "d4a017"), ("yellow", "f9a825"), ("жёлт", "f9a825"), ("желт", "f9a825"),
            ("pink", "e91e63"), ("розов", "e91e63"), ("rose", "e8a3b5"),
            ("purple", "7b1fa2"), ("фиолет", "7b1fa2"), ("violet", "7b4fb0"), ("lavender", "b9a7d8"), ("lilac", "c8a8e0"), ("deep purple", "4a2c6e"),
            ("orange", "e65100"), ("оранж", "e65100"),
        };

        private static string BuildImageUrl(string brand, string title, string color)
        {
            var hex = ResolveColorHex(color);
            var textHex = IsDark(hex) ? "ffffff" : "1a1a1a";
            var text = Uri.EscapeDataString($"{brand} {title}\n{color}");
            return $"https://placehold.co/600x600/{hex}/{textHex}/png?text={text}";
        }

        private static string ResolveColorHex(string color)
        {
            var lower = (color ?? string.Empty).ToLowerInvariant();
            foreach (var (key, hex) in ColorMap)
                if (lower.Contains(key)) return hex;
            return "64748b"; // нейтральный, если цвет не распознан
        }

        private static bool IsDark(string hex)
        {
            if (hex.Length != 6) return true;
            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);
            // Относительная яркость (0..255); < 140 считаем тёмным
            return (0.299 * r + 0.587 * g + 0.114 * b) < 140;
        }

        // Случайный, но воспроизводимый остаток: ~1 из 12 SKU — нет в наличии.
        private int NextStock()
        {
            var roll = _rnd.Next(0, 12);
            return roll == 0 ? 0 : _rnd.Next(3, 60);
        }
    }
}
