using System;
using System.Collections.Generic;

namespace PhoneStore.Data
{
    public class ComponentCategory : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
