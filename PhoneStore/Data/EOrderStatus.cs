namespace PhoneStore.Data;

public enum EOrderStatus
{
    Pending   = 0,   // В ожидании
    Confirmed = 1,   // Подтверждён менеджером
    Shipped   = 2,   // Отправлен
    Delivered = 3,   // Доставлен
    Cancelled = 4    // Отменён
}
