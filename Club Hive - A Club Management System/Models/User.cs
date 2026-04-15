namespace ClubHive.Models
{
    public enum UserRank
    {
        Student,
        ClubExecutive,
        Admin
    }

    public class User
    {
        public int Id { get; set; }

        public string ?FirstName { get; set; }
        
        public string ?LastName { get; set; }

        public string ?Email { get; set; }

        public string ?Password { get; set; }

        public UserRank Rank { get; set; } = UserRank.Student;

        public List<Club>? JoinedClubs { get; set; }
    }
}
