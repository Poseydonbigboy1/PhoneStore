using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public String Title { get; set; } = String.Empty;
        public String Description { get; set; } = String.Empty;
    }
}