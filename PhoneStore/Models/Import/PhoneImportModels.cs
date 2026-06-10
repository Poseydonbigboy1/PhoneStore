using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhoneStore.Models.Import
{
    /// <summary>Одна модель телефона из статического датасета (Data/Import/phones-dataset.json).</summary>
    public class PhoneModelDto
    {
        public string Brand { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Os { get; set; } = string.Empty;
        public double ScreenSize { get; set; }
        public string ScreenType { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public double RefreshRate { get; set; }
        public string Chipset { get; set; } = string.Empty;
        public double MainCamera { get; set; }
        public double FrontCamera { get; set; }
        public double Battery { get; set; }
        public bool Nfc { get; set; }
        public bool FiveG { get; set; }
        public string Bluetooth { get; set; } = string.Empty;
        public bool WirelessCharging { get; set; }
        public string IpRating { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public double Weight { get; set; }

        /// <summary>Базовая цена в USD (для младшего объёма памяти).</summary>
        public double BaseUsd { get; set; }
        /// <summary>Скидка в процентах (0..100), применяется ко всем SKU модели.</summary>
        public double Discount { get; set; }

        public List<StorageTierDto> Storage { get; set; } = new();
        public List<string> Colors { get; set; } = new();

        /// <summary>
        /// Необязательные реальные URL изображений товара. Если заданы — используются как есть
        /// (одинаковые для всех SKU). Если пусто — генерируется плейсхолдер под цвет каждого SKU.
        /// </summary>
        public List<string> Images { get; set; } = new();
    }

    /// <summary>Вариант объёма памяти: определяет цену-надбавку и объём ОЗУ.</summary>
    public class StorageTierDto
    {
        public int Gb { get; set; }
        public int RamGb { get; set; }
        /// <summary>Надбавка к BaseUsd за этот объём, в USD.</summary>
        public double AddUsd { get; set; }
    }

    /// <summary>Сводка результата импорта — что и сколько создано.</summary>
    public class ImportSummary
    {
        public int BrandsCreated { get; set; }
        public int CategoriesCreated { get; set; }
        public int ComponentsCreated { get; set; }
        public int ProductsCreated { get; set; }
        public int SkusCreated { get; set; }
        public int ProductComponentsCreated { get; set; }
        public int ProductsSkipped { get; set; }
        public double UsdToRubRate { get; set; }
        public List<string> Notes { get; set; } = new();
    }
}
