using ClubHive.Data;
using ClubHive.Models;
using ClubHive.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ClubHive.Controllers
{
    public class HomeController : Controller
    {
        private const string SessionUserId = "LoggedInUserId";
        private const string SessionFirstName = "LoggedInFirstName";
        private const string SessionUserRank = "LoggedInUserRank";
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ClubExecutiveSignUp()
        {
            return View(new ClubExecutiveSignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClubExecutiveSignUp(ClubExecutiveSignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailExists = await _dbContext.Users
                .AnyAsync(user => user.Email != null && user.Email.ToLower() == model.Email.ToLower());

            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
                return View(model);
            }

            var clubExecutive = new ClubExecutive
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = model.Email.Trim(),
                Password = model.Password
            };

            _dbContext.ClubExecutives.Add(clubExecutive);
            await _dbContext.SaveChangesAsync();

            HttpContext.Session.SetInt32(SessionUserId, clubExecutive.Id);
            HttpContext.Session.SetString(SessionFirstName, clubExecutive.FirstName ?? "User");
            HttpContext.Session.SetString(SessionUserRank, UserRank.ClubExecutive.ToString());

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            var userId = HttpContext.Session.GetInt32(SessionUserId);
            if (userId is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user is null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["LoginError"] = "Please enter your email and password.";
                return RedirectToAction(nameof(Index));
            }

            var normalizedEmail = email.Trim().ToLower();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Email != null &&
                u.Password != null &&
                u.Email.ToLower() == normalizedEmail &&
                u.Password == password);

            if (user is null)
            {
                TempData["LoginError"] = "Invalid email or password.";
                return RedirectToAction(nameof(Index));
            }

            // Requested hardcoded behavior: this account is always a Club Executive.
            if (normalizedEmail == "a@email.com" && user.Rank != UserRank.ClubExecutive)
            {
                user.Rank = UserRank.ClubExecutive;
                await _dbContext.SaveChangesAsync();
            }

            HttpContext.Session.SetInt32(SessionUserId, user.Id);
            HttpContext.Session.SetString(SessionFirstName, user.FirstName ?? "User");
            HttpContext.Session.SetString(SessionUserRank, user.Rank.ToString());

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SignOut")]
        public IActionResult SignOutUser()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
