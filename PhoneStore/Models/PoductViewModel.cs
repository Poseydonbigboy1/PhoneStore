using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhoneStore.Data;

namespace PhoneStore.Models
{
    public class PoductViewModel
    {
        public Guid ProductId { get; set; }
        public Guid SkuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BrandTitle { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public string? ImageUrl { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public int Amount { get; set; }
        public List<ComponentViewModel> Components { get; set; } = new List<ComponentViewModel>();
    }

    public class ComponentViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EDataType DataType { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}