using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<Component> CreateComponents()
        {
            return new List<Component>()
            {
                new Component()
                {
                    Title = "Цвет",
                    Description = "Цвет корпуса устройства",
                    DataType = EDataType.STRING
                },
                new Component()
                {
                    Title = "ОЗУ",
                    Description = "Объем оперативной памяти (ГБ)",
                    DataType = EDataType.INT
                },
                new Component()
                {
                    Title = "Встроенная память",
                    Description = "Объем накопителя данных (ГБ/ТБ)",
                    DataType = EDataType.INT
                },
                new Component()
                {
                    Title = "Процессор",
                    Description = "Модель и частота центрального процессора",
                    DataType = EDataType.STRING
                },
                new Component()
                {
                    Title = "Диагональ экрана",
                    Description = "Размер дисплея в дюймах",
                    DataType = EDataType.DOUBLE
                },
                new Component()
                {
                    Title = "Тип матрицы",
                    Description = "Технология изготовления экрана (OLED, AMOLED, IPS)",
                    DataType = EDataType.STRING
                },
                new Component()
                {
                    Title = "Емкость аккумулятора",
                    Description = "Объем батареи в мАч",
                    DataType = EDataType.INT
                },
                new Component()
                {
                    Title = "Основная камера",
                    Description = "Разрешение модулей тыловой камеры (Мп)",
                    DataType = EDataType.INT
                },
                new Component()
                {
                    Title = "Фронтальная камера",
                    Description = "Разрешение селфи-камеры (Мп)",
                    DataType = EDataType.INT
                },
                new Component()
                {
                    Title = "Операционная система",
                    Description = "Версия установленной ОС (iOS, Android)",
                    DataType = EDataType.STRING
                },
                new Component()
                {
                    Title = "Поддержка 5G",
                    Description = "Наличие возможности работы в сетях пятого поколения",
                    DataType = EDataType.BOOLEAN
                },
                new Component()
                {
                    Title = "NFC",
                    Description = "Наличие чипа для бесконтактной оплаты",
                    DataType = EDataType.BOOLEAN
                },
                new Component()
                {
                    Title = "Модель",
                    Description = "Модель устройства`",
                    DataType = EDataType.STRING
                },
                new Component()
                {
                    Title = "Assset",
                    Description = "Фото, Видео, 3D модель устройства",
                    DataType = EDataType.STRING
                }
            };
        }
    }
}