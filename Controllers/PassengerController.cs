using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingSystem.Controllers
{
    [Authorize] // All actions require authentication
    public class PassengerController : Controller
    {
        private readonly DatabaseContext _context;

        public PassengerController(DatabaseContext context)
        {
            _context = context;
        }

        // ========== INDEX (ADMIN ONLY) ==========
        // ❌ Only Admin can view all passengers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var passengers = await _context.Passengers.ToListAsync();
            return View(passengers);
        }

        // ========== CREATE (ALL AUTHENTICATED USERS) ==========
        // ✅ Both Admin and Customer can create passengers
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Both Admin and Customer can create passengers - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Passenger passenger)
        {
            if (!ModelState.IsValid)
                return View(passenger);

            if (await _context.Passengers.AnyAsync(p => p.PassportNumber == passenger.PassportNumber))
            {
                ModelState.AddModelError("PassportNumber", "This passport number already exists.");
                return View(passenger);
            }

            // ✅ SET UserId - Link passenger to current user
            passenger.UserId = User.Identity.Name;

            await _context.Passengers.AddAsync(passenger);
            await _context.SaveChangesAsync();

            // Redirect based on role
            if (User.IsInRole("Customer"))
            {
                TempData["SuccessMessage"] = "Passenger created successfully!";
                return RedirectToAction("Create", "Booking");
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== EDIT (ADMIN ONLY) ==========
        // ❌ Only Admin can edit passengers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();
            return View(passenger);
        }

        // ❌ Only Admin can edit passengers - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Passenger passenger)
        {
            if (!ModelState.IsValid)
                return View(passenger);

            var existingPassenger = await _context.Passengers.FindAsync(passenger.Id);
            if (existingPassenger == null) return NotFound();

            // Check if passport number is being changed to an existing one
            if (existingPassenger.PassportNumber != passenger.PassportNumber)
            {
                if (await _context.Passengers.AnyAsync(p =>
                    p.PassportNumber == passenger.PassportNumber && p.Id != passenger.Id))
                {
                    ModelState.AddModelError("PassportNumber", "This passport number already exists.");
                    return View(passenger);
                }
            }

            // Explicitly update fields to avoid overwriting unintentionally
            existingPassenger.First_Name = passenger.First_Name;
            existingPassenger.Last_Name = passenger.Last_Name;
            existingPassenger.PassportNumber = passenger.PassportNumber;
            existingPassenger.Age = passenger.Age;
            existingPassenger.Nationality = passenger.Nationality;
            existingPassenger.ContactEmail = passenger.ContactEmail;
            existingPassenger.PhoneNumber = passenger.PhoneNumber;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ========== DELETE (ADMIN ONLY) ==========
        // ❌ Only Admin can delete passengers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var passenger = await _context.Passengers
                .Include(p => p.Bookings) // Include bookings to show warning
                .FirstOrDefaultAsync(p => p.Id == id);

            if (passenger == null) return NotFound();
            return View(passenger);
        }

        // ❌ Only Admin can delete passengers - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var passenger = await _context.Passengers
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (passenger == null) return NotFound();

            // Prevent deletion if passenger has bookings
            if (passenger.Bookings != null && passenger.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete passenger with existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ========== DETAILS (ALL AUTHENTICATED USERS) ==========
        // ✅ Both Admin and Customer can view passenger details
        public async Task<IActionResult> Details(int id)
        {
            var passenger = await _context.Passengers
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.Flight)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (passenger == null) return NotFound();

            return View(passenger);
        }
    }
}