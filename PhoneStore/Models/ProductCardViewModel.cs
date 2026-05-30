using System;
using System.Collections.Generic;
using PhoneStore.Data;

namespace PhoneStore.Models
{
    public class ProductCardViewModel
    {
        /// <summary>Уникальный ID товара</summary>
        public Guid ProductId { get; set; }

        /// <summary>Название товара</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Описание товара</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Брэнд товара</summary>
        public string BrandTitle { get; set; } = string.Empty;

        /// <summary>изображения товара (URL)</summary>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>Основной выбранный SKU</summary>
        public SkuCardViewModel MainSku { get; set; } = new SkuCardViewModel();

        /// <summary>Дополнительные SKU-вариации</summary>
        public List<SkuCardViewModel> AdditionalSkus { get; set; } = new List<SkuCardViewModel>();

        /// <summary>Общие компоненты товара (характеристики, которые одинаковые для всех SKU)</summary>
        public List<ComponentViewModel> CommonComponents { get; set; } = new List<ComponentViewModel>();
    }

    public class SkuCardViewModel
    {
        /// <summary>Уникальный ID SKU</summary>
        public Guid SkuId { get; set; }

        /// <summary>Цена</summary>
        public double Price { get; set; }

        /// <summary>Скидка (в % или как абсолютное значение)</summary>
        public double Discount { get; set; }

        /// <summary>Доступное количество на складе</summary>
        public double Amount { get; set; }

        /// <summary>Цена с учётом скидки</summary>
        public double PriceWithDiscount => Discount > 0 ? Price - (Price * Discount / 100) : Price;

        /// <summary>Компоненты, специфичные для этого SKU (память, цвет и т.д.)</summary>
        public List<ComponentViewModel> SkuSpecificComponents { get; set; } = new List<ComponentViewModel>();
    }
}
