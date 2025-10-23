namespace TimezoneApi.Models
{
    public class EventRecord
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public DateTimeOffset SavedUtc { get; set; }  // store UTC instant
        public string SavedTz { get; set; } = "";     // e.g., "Australia/Sydney"
    }
}
