using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        // Метод теперь зависит от категорий
        public static List<Component> CreateComponents(List<ComponentCategory> categories)
        {
            var mainCat = categories.First(c => c.Title == "Основные").Id;
            var screenCat = categories.First(c => c.Title == "Экран").Id;
            var memoryCat = categories.First(c => c.Title == "Память и процессор").Id;
            var cameraCat = categories.First(c => c.Title == "Камера").Id;
            var connectivityCat = categories.First(c => c.Title == "Связь").Id;
            var powerCat = categories.First(c => c.Title == "Питание").Id;

            return new List<Component>
            {
                // Основные
                new Component { Id = Guid.NewGuid(), Title = "Цвет", DataType = EDataType.STRING, ComponentCategoryId = mainCat },
                new Component { Id = Guid.NewGuid(), Title = "Операционная система", DataType = EDataType.STRING, ComponentCategoryId = mainCat },

                // Экран
                new Component { Id = Guid.NewGuid(), Title = "Диагональ экрана", DataType = EDataType.DOUBLE, ComponentCategoryId = screenCat },
                new Component { Id = Guid.NewGuid(), Title = "Тип матрицы", DataType = EDataType.STRING, ComponentCategoryId = screenCat },
                new Component { Id = Guid.NewGuid(), Title = "Разрешение экрана", DataType = EDataType.STRING, ComponentCategoryId = screenCat }, // Новый
                new Component { Id = Guid.NewGuid(), Title = "Частота обновления экрана", DataType = EDataType.DOUBLE, ComponentCategoryId = screenCat }, // Новый

                // Память и процессор
                new Component { Id = Guid.NewGuid(), Title = "ОЗУ", DataType = EDataType.DOUBLE, ComponentCategoryId = memoryCat },
                new Component { Id = Guid.NewGuid(), Title = "Встроенная память", DataType = EDataType.DOUBLE, ComponentCategoryId = memoryCat },
                new Component { Id = Guid.NewGuid(), Title = "Процессор", DataType = EDataType.STRING, ComponentCategoryId = memoryCat },

                // Камера
                new Component { Id = Guid.NewGuid(), Title = "Основная камера", DataType = EDataType.DOUBLE, ComponentCategoryId = cameraCat },
                new Component { Id = Guid.NewGuid(), Title = "Фронтальная камера", DataType = EDataType.DOUBLE, ComponentCategoryId = cameraCat },
                // Связь
                new Component { Id = Guid.NewGuid(), Title = "NFC", DataType = EDataType.BOOLEAN, ComponentCategoryId = connectivityCat },
                new Component { Id = Guid.NewGuid(), Title = "Поддержка 5G", DataType = EDataType.BOOLEAN, ComponentCategoryId = connectivityCat },

                // Питание
                new Component { Id = Guid.NewGuid(), Title = "Емкость аккумулятора", DataType = EDataType.DOUBLE, ComponentCategoryId = powerCat }
            };
        }
    }
}