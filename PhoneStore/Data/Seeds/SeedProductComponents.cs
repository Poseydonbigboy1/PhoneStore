using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<ProductComponent> CreateProductComponents()
        {
            var skus = CreateSkus();
            var components = CreateComponents();
            var productComponents = new List<ProductComponent>();

            // void AddValue(string productTitle, string componentTitle, object value, double price = 0)
            // {
            //     var product = products.FirstOrDefault(p => p.Title == productTitle);
            //     var component = components.FirstOrDefault(c => c.Title == componentTitle);

            //     if (product != null && component != null)
            //     {
            //         productComponents.Add(new ProductComponent
            //         {
            //             Id = Guid.NewGuid(),
            //             ProductId = product.Id, // Предполагается, что в модели Product есть Id
            //             Product = product,
            //             ComponentId = component.Id, // Предполагается, что в модели Component есть Id
            //             Component = component,
            //             Value = new ProductComponentValue { Value = value }
            //         });
            //     }
            // }

            // // --- Характеристики для iPhone 14 Pro Max ---
            // AddValue("iPhone 14 Pro Max", "Цвет", "Deep Purple");
            // AddValue("iPhone 14 Pro Max", "Цвет", "Black");
            // AddValue("iPhone 14 Pro Max", "Цвет", "White");
            // AddValue("iPhone 14 Pro Max", "ОЗУ", 6);
            // AddValue("iPhone 14 Pro Max", "ОЗУ", 8);
            // AddValue("iPhone 14 Pro Max", "ОЗУ", 10);
            // AddValue("iPhone 14 Pro Max", "Встроенная память", 256, 0);
            // AddValue("iPhone 14 Pro Max", "Встроенная память", 512, 0);
            // AddValue("iPhone 14 Pro Max", "Встроенная память", 1024, 0);
            // AddValue("iPhone 14 Pro Max", "Процессор", "A16 Bionic");
            // AddValue("iPhone 14 Pro Max", "Диагональ экрана", 6.7);
            // AddValue("iPhone 14 Pro Max", "Тип матрицы", "OLED");
            // AddValue("iPhone 14 Pro Max", "Емкость аккумулятора", 4323);
            // AddValue("iPhone 14 Pro Max", "Основная камера", 48);
            // AddValue("iPhone 14 Pro Max", "Операционная система", "iOS");
            // AddValue("iPhone 14 Pro Max", "Поддержка 5G", true);
            // AddValue("iPhone 14 Pro Max", "NFC", true);

            // // --- Характеристики для Samsung Galaxy S22 Ultra ---
            // AddValue("Samsung Galaxy S22 Ultra", "Цвет", "Phantom Black");
            // AddValue("Samsung Galaxy S22 Ultra", "ОЗУ", 12);
            // AddValue("Samsung Galaxy S22 Ultra", "Встроенная память", 512);
            // AddValue("Samsung Galaxy S22 Ultra", "Процессор", "Snapdragon 8 Gen 1");
            // AddValue("Samsung Galaxy S22 Ultra", "Диагональ экрана", 6.8);
            // AddValue("Samsung Galaxy S22 Ultra", "Тип матрицы", "AMOLED");
            // AddValue("Samsung Galaxy S22 Ultra", "Емкость аккумулятора", 5000);
            // AddValue("Samsung Galaxy S22 Ultra", "Основная камера", 108);
            // AddValue("Samsung Galaxy S22 Ultra", "Операционная система", "Android");
            // AddValue("Samsung Galaxy S22 Ultra", "Поддержка 5G", true);
            // AddValue("Samsung Galaxy S22 Ultra", "NFC", true);

            return productComponents;
        }
    }
}