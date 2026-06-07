namespace PhoneStore.Data;

public class UserAddress : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Метка: «Дом», «Работа», и т.д.</summary>
    public string Label { get; set; } = "Дом";

    /// <summary>Полный адрес доставки</summary>
    public string Address { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}
