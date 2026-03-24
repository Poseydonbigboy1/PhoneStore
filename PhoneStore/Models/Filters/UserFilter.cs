namespace PhoneStore.Models.Filters
{
    public class UserFilter
    {
        public string? Login { get; set; } = null;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
}
