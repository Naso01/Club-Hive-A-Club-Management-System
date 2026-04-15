using ClubHive.Data;
using ClubHive.Models;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        context.Database.Migrate();

        if (context.Users.Any()) return; // already seeded

        context.Users.AddRange(
            new User { FirstName = "Admin", LastName = "User", Email = "admin@clubhive.local" },
            new User { FirstName = "Jane", LastName = "Doe", Email = "jane@clubhive.local" }
        );

        context.SaveChanges();
    }
}