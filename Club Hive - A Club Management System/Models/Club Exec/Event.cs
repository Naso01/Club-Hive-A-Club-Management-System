namespace ClubHive.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public string? Room { get; set; }
        public List<string>? Sponsors { get; set; }
    }
}
