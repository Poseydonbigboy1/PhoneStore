using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data
{
    public class Component
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public String Title { get; set; } = String.Empty;
        public String Description { get; set; } = String.Empty;
        public EDataType DataType { get; set; }
        // Если понадобится делать ограничения для типов jsonb
        // public DataTypeMeta DataTypeMeta { get; set; }
        public ICollection<ProductComponent> ProductComponents { get; set; } = new List<ProductComponent>();
    }

    public enum EDataType
    {
        STRING,
        INT,
        DOUBLE,
        BOOLEAN
    }
}
