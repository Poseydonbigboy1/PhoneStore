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
        public string ValueJson { get; set; } = "null";

        [NotMapped]
        public object? Value
        {
            get
            {
                return JsonSerializer.Deserialize<object>(ValueJson);
            }
            set => ValueJson = JsonSerializer.Serialize(value);
        }
    }
}