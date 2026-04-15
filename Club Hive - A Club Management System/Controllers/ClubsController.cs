using ClubHive.Data;
using ClubHive.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubHive.Controllers
{
    public class ClubsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ClubsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Club());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Club club)
        {
            if (string.IsNullOrWhiteSpace(club.Name))
            {
                ModelState.AddModelError(nameof(club.Name), "Club name is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(club);
            }

            var clubExecutive = await _dbContext.ClubExecutives
                .Include(executive => executive.ManagedClubs)
                .FirstOrDefaultAsync();

            if (clubExecutive == null)
            {
                ModelState.AddModelError(string.Empty, "No club executive account exists to manage this club.");
                return View(club);
            }

            clubExecutive.ManagedClubs ??= new List<Club>();
            club.Executives ??= new List<ClubExecutive>();

            clubExecutive.ManagedClubs.Add(club);
            club.Executives.Add(clubExecutive);

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Club saved and added to managed clubs.";
            return RedirectToAction(nameof(Create));
        }
    }
}
