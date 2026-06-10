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

            // SKU-специфичные характеристики iPhone 15 Pro
            AddComponentToSku(iphone256gb.Id, "Встроенная память", 256);
            AddComponentToSku(iphone256gb.Id, "ОЗУ", 8);
            AddComponentToSku(iphone256gb.Id, "Цвет", "Natural Titanium");

            AddComponentToSku(iphone512gb.Id, "Встроенная память", 512);
            AddComponentToSku(iphone512gb.Id, "ОЗУ", 8);
            AddComponentToSku(iphone512gb.Id, "Цвет", "Blue Titanium");

            // Общие характеристики iPhone 15 Pro (одинаковые для всех SKU)
            foreach (var sku in iphoneSkus)
            {
                AddComponentToSku(sku.Id, "Процессор", "A17 Pro");
                AddComponentToSku(sku.Id, "Диагональ экрана", 6.1);
                AddComponentToSku(sku.Id, "Тип матрицы", "Super Retina XDR (OLED)");
                AddComponentToSku(sku.Id, "Разрешение экрана", "2556x1179");
                AddComponentToSku(sku.Id, "Частота обновления экрана", 120);
                AddComponentToSku(sku.Id, "Основная камера", 48);
                AddComponentToSku(sku.Id, "Фронтальная камера", 12);
                AddComponentToSku(sku.Id, "Емкость аккумулятора", 3274);
                AddComponentToSku(sku.Id, "NFC", true);
                AddComponentToSku(sku.Id, "Поддержка 5G", true);
                AddComponentToSku(sku.Id, "Операционная система", "iOS 17");
            }

            // --- Samsung Galaxy S25 Ultra ---
            var samsungS25Id = productsDict["Samsung Galaxy S25 Ultra"];
            var samsungSkus = skus.Where(s => s.ProductId == samsungS25Id).ToList();

            var samsung512gb = samsungSkus.First(s => s.Price == 1299.00);
            var samsung1tb    = samsungSkus.First(s => s.Price == 1499.00);

            // SKU-специфичные характеристики Samsung
            AddComponentToSku(samsung512gb.Id, "Встроенная память", 512);
            AddComponentToSku(samsung512gb.Id, "ОЗУ", 12);
            AddComponentToSku(samsung512gb.Id, "Цвет", "Phantom Black");

            AddComponentToSku(samsung1tb.Id, "Встроенная память", 1024);
            AddComponentToSku(samsung1tb.Id, "ОЗУ", 16);
            AddComponentToSku(samsung1tb.Id, "Цвет", "Phantom Silver");

            // Общие характеристики Samsung (одинаковые для всех SKU)
            foreach (var sku in samsungSkus)
            {
                AddComponentToSku(sku.Id, "Процессор", "Snapdragon 8 Gen 3 for Galaxy");
                AddComponentToSku(sku.Id, "Диагональ экрана", 6.8);
                AddComponentToSku(sku.Id, "Тип матрицы", "Dynamic AMOLED 2X");
                AddComponentToSku(sku.Id, "Разрешение экрана", "3120x1440");
                AddComponentToSku(sku.Id, "Частота обновления экрана", 120);
                AddComponentToSku(sku.Id, "Основная камера", 200);
                AddComponentToSku(sku.Id, "Фронтальная камера", 12);
                AddComponentToSku(sku.Id, "Емкость аккумулятора", 5000);
                AddComponentToSku(sku.Id, "NFC", true);
                AddComponentToSku(sku.Id, "Поддержка 5G", true);
                AddComponentToSku(sku.Id, "Операционная система", "Android 14");
            }

            return productComponents;
        }
    }
}