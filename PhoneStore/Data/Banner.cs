namespace PhoneStore.Data;

public class Banner : IEntity
{
    public Guid   Id        { get; set; } = Guid.NewGuid();
    public string ImageUrl  { get; set; } = string.Empty;
    public string Link      { get; set; } = string.Empty;   // необязательная ссылка по клику
    public int    SortOrder { get; set; } = 0;
    public bool   IsActive  { get; set; } = true;
}
