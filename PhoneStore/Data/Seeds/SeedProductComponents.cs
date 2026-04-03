using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<ProductComponent> CreateProductComponents(List<Product> products, List<Sku> skus, List<Component> components)
        {
            // Получаем SKU и компоненты, созданные в других файлах сидов
            var componentsDict = components.ToDictionary(c => c.Title, c => c.Id);
            var productsDict = products.ToDictionary(p => p.Title, p => p.Id);

            var productComponents = new List<ProductComponent>();

            // Вспомогательный метод, чтобы не дублировать код
            // Он находит компонент по имени и добавляет его к указанному SKU
            void AddComponentToSku(Guid skuId, string componentTitle, object value)
            {
                if (componentsDict.TryGetValue(componentTitle, out var componentId))
                {
                    productComponents.Add(new ProductComponent
                    {
                        Id = Guid.NewGuid(),
                        SkuId = skuId,
                        ComponentId = componentId,
                        // Предполагается, что у вас есть модель-обертка для значения
                        Value =  value
                    });
                }
            }

            // --- Описываем характеристики для SKU "iPhone 15 Pro" ---
            var iphone15ProId = productsDict["iPhone 15 Pro"];
            var iphoneSkus = skus.Where(s => s.ProductId == iphone15ProId).ToList();

            // Находим конкретные SKU по их ценам (предполагаем, что они уникальны для продукта)
            var iphone256gb = iphoneSkus.First(s => s.Price == 1199.00);
            var iphone512gb = iphoneSkus.First(s => s.Price == 1399.00);

            // Добавляем характеристики для SKU с 256GB
            AddComponentToSku(iphone256gb.Id, "Встроенная память", 256);
            AddComponentToSku(iphone256gb.Id, "ОЗУ", 8);
            AddComponentToSku(iphone256gb.Id, "Цвет", "Natural Titanium");

            // Добавляем характеристики для SKU с 512GB
            AddComponentToSku(iphone512gb.Id, "Встроенная память", 512);
            AddComponentToSku(iphone512gb.Id, "ОЗУ", 8);
            AddComponentToSku(iphone512gb.Id, "Цвет", "Blue Titanium");

            // --- Описываем характеристики для SKU "Samsung Galaxy S25 Ultra" ---
            var samsungS25Id = productsDict["Samsung Galaxy S25 Ultra"];
            var samsungSkus = skus.Where(s => s.ProductId == samsungS25Id).ToList();

            var samsung512gb = samsungSkus.First(s => s.Price == 1299.00);
            var samsung1tb = samsungSkus.First(s => s.Price == 1499.00);

            // Добавляем характеристики для SKU с 512GB
            AddComponentToSku(samsung512gb.Id, "Встроенная память", 512);
            AddComponentToSku(samsung512gb.Id, "ОЗУ", 12);
            AddComponentToSku(samsung512gb.Id, "Цвет", "Phantom Black");

            // Добавляем характеристики для SKU с 1TB
            AddComponentToSku(samsung1tb.Id, "Встроенная память", 1024); // 1TB
            AddComponentToSku(samsung1tb.Id, "ОЗУ", 16);
            AddComponentToSku(samsung1tb.Id, "Цвет", "Phantom Silver");


            // Добавляем ОБЩИЕ характеристики, которые одинаковы для всех SKU одного товара
            foreach (var sku in iphoneSkus)
            {
                AddComponentToSku(sku.Id, "Процессор", "A17 Pro");
                AddComponentToSku(sku.Id, "Диагональ экрана", 6.1);
            }

            foreach (var sku in samsungSkus)
            {
                AddComponentToSku(sku.Id, "Процессор", "Snapdragon 8 Gen 4");
                AddComponentToSku(sku.Id, "Диагональ экрана", 6.8);
            }

            // --- Общие характеристики для iPhone 15 Pro ---
            var iphoneProduct = products.First(p => p.Title == "iPhone 15 Pro");
            iphoneSkus = skus.Where(s => s.ProductId == iphoneProduct.Id).ToList();
            
            // Добавляем общие характеристики, которые одинаковы для всех вариаций iPhone 15 Pro
            foreach (var sku in iphoneSkus)
            {
                productComponents.AddRange(new List<ProductComponent>
                {
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Процессор").Id, SkuId = sku.Id, Value = "A17 Pro" },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Диагональ экрана").Id, SkuId = sku.Id, Value = 6.1 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Тип матрицы").Id, SkuId = sku.Id, Value = "Super Retina XDR (OLED)" },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Разрешение экрана").Id, SkuId = sku.Id, Value = "2556x1179" },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Частота обновления экрана").Id, SkuId = sku.Id, Value = 120 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Основная камера").Id, SkuId = sku.Id, Value = 48 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Фронтальная камера").Id, SkuId = sku.Id, Value = 12 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Емкость аккумулятора").Id, SkuId = sku.Id, Value = 3274 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "NFC").Id, SkuId = sku.Id, Value = true },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Поддержка 5G").Id, SkuId = sku.Id, Value = true },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Операционная система").Id, SkuId = sku.Id, Value = "iOS 17" }
                });
            }

            // --- Уникальные характеристики для каждой вариации iPhone (память, цвет, ОЗУ) ---
            // ... (существующий код для уникальных характеристик iPhone)

            // --- Общие характеристики для Samsung Galaxy S25 Ultra ---
            var samsungProduct = products.First(p => p.Title == "Samsung Galaxy S25 Ultra");
            samsungSkus = skus.Where(s => s.ProductId == samsungProduct.Id).ToList();
            
            // Добавляем общие характеристики для всех вариаций Samsung
            foreach (var sku in samsungSkus)
            {
                productComponents.AddRange(new List<ProductComponent>
                {
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Процессор").Id, SkuId = sku.Id, Value = "Snapdragon 8 Gen 3 for Galaxy" },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Диагональ экрана").Id, SkuId = sku.Id, Value = 6.8 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Тип матрицы").Id, SkuId = sku.Id, Value = "Dynamic AMOLED 2X" },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Разрешение экрана").Id, SkuId = sku.Id, Value = "3120x1440" },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Частота обновления экрана").Id, SkuId = sku.Id, Value = 120 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Основная камера").Id, SkuId = sku.Id, Value = 200 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Фронтальная камера").Id, SkuId = sku.Id, Value = 12 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Емкость аккумулятора").Id, SkuId = sku.Id, Value = 5000 },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "NFC").Id, SkuId = sku.Id, Value = true },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Поддержка 5G").Id, SkuId = sku.Id, Value = true },
                    new ProductComponent { ComponentId = components.First(c => c.Title == "Операционная система").Id, SkuId = sku.Id, Value = "Android 14" }
                });
            }

            return productComponents;
        }
    }
}