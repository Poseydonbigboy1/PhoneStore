using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhoneStore.Data;

namespace PhoneStore.Models
{
    public class PoductViewModel
    {
        public string Title { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public double Price { get; set; }
        public double Discount { get; set; }
        public List<ComponentViewModel> Components { get; set; } = new List<ComponentViewModel>();
    }

    public class ComponentViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public EDataType DataType { get; set; }
    }
}