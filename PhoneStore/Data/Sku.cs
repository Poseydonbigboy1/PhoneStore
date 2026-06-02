using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data
{
    public class Sku : IEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid ProductId { get; set; }
        // public Guid ProductComponentId { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double Discount { get; set; }

        public Product Product { get; set; } = null!;
        // public ProductComponent ProductComponent { get; set; }
    }
}
