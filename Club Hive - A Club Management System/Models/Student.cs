namespace ClubHive.Models
{
    public class Student : User
    {
        public Student()
        {
            Rank = UserRank.Student;
        }
    }
}
