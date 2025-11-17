using Microsoft.AspNetCore.Mvc;
using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Controllers
{
    public class PassengerController : Controller
    {
        private readonly DatabaseContext _context;

        public PassengerController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Passenger
        public async Task<IActionResult>  Index()
        {
            var passengers = _context.Passengers.ToList();
            return View(passengers);
        }

        // GET: Passenger/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Passenger/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Passenger passenger)
        {
            if (!ModelState.IsValid)
                return View(passenger);

            // Additional server-side validation example
            if (_context.Passengers.Any(p => p.PassportNumber == passenger.PassportNumber))
            {
                ModelState.AddModelError("PassportNumber", "This passport number already exists.");
                return View(passenger);
            }

            await _context.Passengers.AddAsync(passenger);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Passenger/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();

            return View(passenger);
        }

        // POST: Passenger/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Passenger passenger)
        {
            if (!ModelState.IsValid)
                return View(passenger);

            var existingPassenger = await _context.Passengers.FindAsync(passenger.Id);
            if (existingPassenger == null) return NotFound();

            // Explicitly update fields to avoid overwriting unintentionally
            existingPassenger.First_Name = passenger.First_Name;
            existingPassenger.Last_Name = passenger.Last_Name;
            existingPassenger.PassportNumber = passenger.PassportNumber;
            existingPassenger.Age = passenger.Age;
            existingPassenger.ContactEmail = passenger.ContactEmail;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Passenger/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();

            return View(passenger);
        }

        // POST: Passenger/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();

            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Passenger/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();

            return View(passenger);
        }
    }
}