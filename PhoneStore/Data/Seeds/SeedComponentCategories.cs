using System;
using System.Collections.Generic;

namespace PhoneStore.Data.Seeds
{
    public static class SeedComponentCategories
    {
        public static List<ComponentCategory> CreateComponentCategories()
        {
            return new List<ComponentCategory>
            {
                new ComponentCategory { Id = Guid.NewGuid(), Title = "Основные" },
                new ComponentCategory { Id = Guid.NewGuid(), Title = "Экран" },
                new ComponentCategory { Id = Guid.NewGuid(), Title = "Память и процессор" },
                new ComponentCategory { Id = Guid.NewGuid(), Title = "Камера" },
                new ComponentCategory { Id = Guid.NewGuid(), Title = "Связь" },
                new ComponentCategory { Id = Guid.NewGuid(), Title = "Питание" }
            };
        }
    }
}
