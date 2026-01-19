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

        public AuthController(DatabaseContext db)
        {
            _db = db;
        }

        // ==========================================
        // LOGIN ACTIONS
        // ==========================================

        [HttpGet]
        public IActionResult Login()
        {
            // Agar user pehle se login hai toh usay dashboard bhej dein
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Username check karein
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "Username not Found. First register .");
                return View(model);
            }

            // 2. Password Verify karein (Specific Notification added here)
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.HashedPassword))
            {
                // UI par error dikhane ke liye
                ModelState.AddModelError("Password", "Wrong Password Try Again.");

                // Top notification bar ke liye
                TempData["ErrorMessage"] = "Authentication Failed: Incorrect Password!";

                return View(model);
            }

            // 3. Token Generate karein
            var token = GenerateToken(user);

            // 4. Cookie set karein
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            TempData["SuccessMessage"] = $"Khush Amdeed, {user.UserName}!";

            // 5. Role based redirection
            return user.Role == "Admin"
                ? RedirectToAction("AdminDashboard", "Dashboard")
                : RedirectToAction("CustomerDashboard", "Dashboard");
        }

        // ==========================================
        // REGISTER ACTIONS
        // ==========================================

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // Username uniqueness check
            if (await _db.Users.AnyAsync(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Already taken.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

        
                user.Role = "Customer";
                TempData["SuccessMessage"] = "Registration successful ! Please login now .";
            

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // ==========================================
        // LOGOUT
        // ==========================================

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            TempData["InfoMessage"] = "Successful logout :/.";
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // JWT HELPER
        // ==========================================

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Security Note: Use a more secure way to store keys in production (e.g., AppSettings)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("class-work-5E-your-super-secret-key-minimum-32-characters"));
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