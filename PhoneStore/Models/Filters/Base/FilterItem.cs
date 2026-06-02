namespace PhoneStore.Models.Filters.Base
{
    public class FilterItem
    {
        public string MatchMode { get; set; } = "Equals";
        public string Value { get; set; } = string.Empty;
    }
}