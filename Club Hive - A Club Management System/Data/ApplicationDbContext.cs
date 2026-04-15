using System.Text.Json;
using ClubHive.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClubHive.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<ClubExecutive> ClubExecutives => Set<ClubExecutive>();
        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<Event> Events => Set<Event>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var sponsorsConverter = new ValueConverter<List<string>?, string>(
                sponsors => JsonSerializer.Serialize(sponsors ?? new List<string>(), (JsonSerializerOptions?)null),
                value => string.IsNullOrWhiteSpace(value)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>());

            var sponsorsComparer = new ValueComparer<List<string>?>(
                (left, right) => (left ?? new List<string>()).SequenceEqual(right ?? new List<string>()),
                value => value == null ? 0 : value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                value => value == null ? new List<string>() : value.ToList());

            modelBuilder.Entity<Event>()
                .Property(eventItem => eventItem.Sponsors)
                .HasConversion(sponsorsConverter)
                .Metadata.SetValueComparer(sponsorsComparer);

            modelBuilder.Entity<User>()
                .Property(user => user.Rank)
                .HasConversion(
                    rank => rank == UserRank.ClubExecutive
                        ? "Club Executive"
                        : rank == UserRank.Admin
                            ? "Admin"
                            : "Student",
                    value => value == "Club Executive"
                        ? UserRank.ClubExecutive
                        : value == "Admin"
                            ? UserRank.Admin
                            : UserRank.Student);

            modelBuilder.Entity<ClubExecutive>().HasData(
                new ClubExecutive
                {
                    Id = 1,
                    FirstName = "Nathan",
                    LastName = "Serrano",
                    Email = "a@email.com",
                    Password = "123",
                    Rank = UserRank.ClubExecutive
                });
        }
    }
}
