using ClubHive.Data;
using ClubHive.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ClubHive.Controllers
{
    public class ClubsController : Controller
    {
        private const string SessionUserId = "LoggedInUserId";
        private const string SessionUserRank = "LoggedInUserRank";
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClubsController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clubs = await _dbContext.Clubs
                .AsNoTracking()
                .OrderBy(club => club.Name)
                .ToListAsync();

            return View(clubs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Club());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var clubExecutive = await GetLoggedInClubExecutiveAsync(includeManagedClubs: true);
            if (clubExecutive == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var managedClub = clubExecutive.ManagedClubs?.FirstOrDefault(club => club.Id == id);
            if (managedClub == null)
            {
                return RedirectToAction(nameof(Manage));
            }

            return View("Create", managedClub);
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var clubExecutive = await GetLoggedInClubExecutiveAsync(includeManagedClubs: true);
            if (clubExecutive == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(clubExecutive.ManagedClubs ?? new List<Club>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Club club, IFormFile? uploadedImage)
        {
            if (string.IsNullOrWhiteSpace(club.Name))
            {
                ModelState.AddModelError(nameof(club.Name), "Club name is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(club);
            }

            var clubExecutive = await GetLoggedInClubExecutiveAsync(includeManagedClubs: true);
            if (clubExecutive == null)
            {
                ModelState.AddModelError(string.Empty, "Please sign in as a club executive to create a club.");
                return View(club);
            }

            clubExecutive.ManagedClubs ??= new List<Club>();

            if (club.Id > 0)
            {
                var existingClub = clubExecutive.ManagedClubs.FirstOrDefault(managedClub => managedClub.Id == club.Id);
                if (existingClub == null)
                {
                    ModelState.AddModelError(string.Empty, "You can only edit clubs that you manage.");
                    return View(club);
                }

                existingClub.Name = club.Name?.Trim();
                existingClub.Description = club.Description?.Trim();

                if (uploadedImage is not null && uploadedImage.Length > 0)
                {
                    existingClub.ImageUrl = await SaveUploadedImageAsync(uploadedImage);
                }
                else
                {
                    existingClub.ImageUrl = club.ImageUrl?.Trim();
                }

                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Club updated successfully.";
                return RedirectToAction(nameof(Manage));
            }

            club.Executives ??= new List<ClubExecutive>();
            club.Name = club.Name?.Trim();
            club.Description = club.Description?.Trim();
            if (uploadedImage is not null && uploadedImage.Length > 0)
            {
                club.ImageUrl = await SaveUploadedImageAsync(uploadedImage);
            }
            else
            {
                club.ImageUrl = club.ImageUrl?.Trim();
            }

            _dbContext.Clubs.Add(club);
            clubExecutive.ManagedClubs.Add(club);
            club.Executives.Add(clubExecutive);

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Club saved and added to managed clubs.";
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var clubExecutive = await GetLoggedInClubExecutiveAsync(includeManagedClubs: true);
            if (clubExecutive == null || clubExecutive.ManagedClubs == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var managedClub = clubExecutive.ManagedClubs.FirstOrDefault(club => club.Id == id);
            if (managedClub == null)
            {
                TempData["SuccessMessage"] = "Club not found in your managed list.";
                return RedirectToAction(nameof(Manage));
            }

            clubExecutive.ManagedClubs.Remove(managedClub);
            _dbContext.Clubs.Remove(managedClub);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Club deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }

        private async Task<ClubExecutive?> GetLoggedInClubExecutiveAsync(bool includeManagedClubs)
        {
            var userId = HttpContext.Session.GetInt32(SessionUserId);
            var userRank = HttpContext.Session.GetString(SessionUserRank);

            if (userId is null || userRank != UserRank.ClubExecutive.ToString())
            {
                return null;
            }

            var query = _dbContext.ClubExecutives.AsQueryable();

            if (includeManagedClubs)
            {
                query = query.Include(executive => executive.ManagedClubs);
            }

            return await query.FirstOrDefaultAsync(executive => executive.Id == userId.Value);
        }

        private async Task<string> SaveUploadedImageAsync(IFormFile uploadedImage)
        {
            var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "clubs");
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(uploadedImage.FileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await uploadedImage.CopyToAsync(stream);

            return $"/uploads/clubs/{fileName}";
        }
    }
}
