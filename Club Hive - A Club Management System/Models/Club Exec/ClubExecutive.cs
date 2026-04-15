namespace ClubHive.Models
{
    public class ClubExecutive : User
    {
        public ClubExecutive()
        {
            Rank = UserRank.ClubExecutive;
        }

        public List<Club>? ManagedClubs { get; set; }
    }
}
