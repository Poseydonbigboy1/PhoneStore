using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data
{
    public class ProductComponent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SkuId { get; set; }
        public Guid ComponentId { get; set; }

        public Sku? Sku { get; set; }
        public Component? Component { get; set; }
        [Required]
        public string ValueJson { get; set; } = JsonSerializer.Serialize(new ProductComponentValue());

        [NotMapped]
        public ProductComponentValue Value
        {
            get => JsonSerializer.Deserialize<ProductComponentValue>(ValueJson) ?? new ProductComponentValue();
            set => ValueJson = JsonSerializer.Serialize(value);
        }
    }

    public class ProductComponentValue
    {
        public object? Value { get; set; }
        public double Price { get; set; } = 0;
    }
}