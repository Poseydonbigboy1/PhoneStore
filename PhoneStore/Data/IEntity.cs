using System;

namespace PhoneStore.Data
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
