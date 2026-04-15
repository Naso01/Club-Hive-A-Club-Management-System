namespace ClubHive.Models
{
    public class Admin : User
    {
        public Admin()
        {
            Rank = UserRank.Admin;
        }
    }
}
