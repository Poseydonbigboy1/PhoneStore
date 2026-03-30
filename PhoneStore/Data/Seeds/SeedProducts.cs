using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data.Seeds
{
    public static partial class SeedData
    {
        public static List<Product> CreateProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Title = "iPhone 15 Pro",
                    Description = "iPhone 15 Pro - это флагманский смартфон от Apple, который был представлен в сентябре 2024 года. Он оснащен 6,1-дюймовым дисплеем Super Retina XDR с технологией ProMotion. Устройство работает на новом процессоре A17 Pro, который обеспечивает непревзойденную производительность. iPhone 15 Pro также имеет тройную камеру с улучшенными возможностями съемки в условиях низкой освещенности и телеобъективом с 5-кратным оптическим зумом. Корпус выполнен из титана, что делает его легче и прочнее."
                },
                new Product()
                {
                    Title = "Samsung Galaxy S25 Ultra",
                    Description = "Samsung Galaxy S25 Ultra - это флагманский смартфон от Samsung, представленный в феврале 2025 года. Он оснащен 6,8-дюймовым дисплеем Dynamic AMOLED 2X. Устройство работает на процессоре Snapdragon 8 Gen 4, который обеспечивает высочайшую производительность. Samsung Galaxy S25 Ultra имеет систему из четырех камер с основным сенсором на 200 Мп и поддержкой 10-кратного оптического зума. Он поддерживает 5G-сети и имеет емкую батарею с поддержкой сверхбыстрой зарядки."
                }
            };
        }
    }
}