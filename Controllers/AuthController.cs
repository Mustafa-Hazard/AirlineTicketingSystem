using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.ViewModels;
using AirlineTicketingSystem.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AirlineTicketingSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly IConfiguration _config;

        public AuthController(DatabaseContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Username not found. Please register first.");
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.HashedPassword))
            {
                ModelState.AddModelError("Password", "Incorrect password. Please try again.");
                TempData["ErrorMessage"] = "Authentication Failed: Incorrect Password!";
                return View(model);
            }

            var token = GenerateToken(user);
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            TempData["SuccessMessage"] = $"Welcome back, {user.UserName}!";

            return user.Role == "Admin"
                ? RedirectToAction("AdminDashboard", "Dashboard")
                : RedirectToAction("CustomerDashboard", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _db.Users.AnyAsync(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Username already taken.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Customer"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please login now.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        private string GenerateToken(User user)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                ?? _config["Jwt:Key"]
                ?? "class-work-5E-your-super-secret-key-minimum-32-characters";
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? _config["Jwt:Issuer"]
                ?? "https://aeronexa.up.railway.app/";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                ?? _config["Jwt:Audience"]
                ?? "https://aeronexa.up.railway.app/";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
