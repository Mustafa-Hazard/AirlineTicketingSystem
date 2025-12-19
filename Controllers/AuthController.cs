using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.ViewModels;
using AirlineTicketingSystem.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AirlineTicketingSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseContext _db;

        public AuthController(DatabaseContext db)
        {
            _db = db;
        }

        // GET: Login
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user exists
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.HashedPassword))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // Generate JWT token
            var token = GenerateToken(user);

            // Set cookie
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            TempData["SuccessMessage"] = $"Welcome back, {user.UserName}!";

            // Redirect based on role
            if (user.Role == "Admin")
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            else
            {
                return RedirectToAction("CustomerDashboard", "Dashboard");
            }
        }

        // GET: Register
        public IActionResult Register()
        {
            // If already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if username already exists
            if (await _db.Users.AnyAsync(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists. Please choose another.");
                return View(model);
            }

            // Create new user
            var user = new User
            {
                UserName = model.UserName,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            // First user becomes Admin, all others become Customer
            if (!await _db.Users.AnyAsync())
            {
                user.Role = "Admin";
                TempData["SuccessMessage"] = "Admin account created successfully! Please login.";
            }
            else
            {
                user.Role = "Customer";
                TempData["SuccessMessage"] = "Registration successful! Please login.";
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        // Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        // Generate JWT Token
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                "class-work-5E-your-super-secret-key-minimum-32-characters"
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7175/",
                audience: "https://localhost:7175/",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}