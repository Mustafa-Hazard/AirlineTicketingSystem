using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace AirlineTicketingSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseContext _db;

        public AuthController(DatabaseContext db)
        {
            _db = db;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var loggedInUser = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (loggedInUser == null)
            {
                ViewBag.ErroMessage = "Incorrect Username!";
                return View(user);
            }

            var passwordOk = BCrypt.Net.BCrypt.Verify(
                user.HashedPassword,
                loggedInUser.HashedPassword
            );

            if (!passwordOk)
            {
                ViewBag.ErroMessage = "Incorrect Password!";
                return View(user);
            }

            // ✅ THIS MUST EXIST:
            HttpContext.Session.SetInt32("UserId", loggedInUser.Id);
            HttpContext.Session.SetString("UserName", loggedInUser.UserName);
            HttpContext.Session.SetString("UserRole", loggedInUser.Role ?? "Customer");

            return RedirectToAction("Index", "Home");
        }


        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (_db.Users.Any(u => u.UserName == user.UserName))
            {
                ViewBag.ErroMessage = "Username already exists! Please try another Username";
                return View(user);
            }

            user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.HashedPassword);

            // 👇 First user = Admin, others = Customer
            if (!_db.Users.Any())
            {
                user.Role = "Admin";
            }
            else
            {
                user.Role = "Customer";
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login", "Auth");
        }


        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            // clear all session keys
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
