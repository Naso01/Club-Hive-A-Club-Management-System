namespace ClubHive.Models
{
    public class Club
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public List<ClubExecutive>? Executives { get; set; }
        public List<Event>? Events { get; set; }
    }
}
