using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingSystem.Controllers
{
    public class FlightsController : Controller
    {
        private readonly DatabaseContext _context;

        public FlightsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            return View(await _context.Flights.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null) return NotFound();

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flight flight)
        {
            // Custom validation
            if (flight.ArrivalTime <= flight.DepartureTime)
                ModelState.AddModelError("ArrivalTime", "ArrivalTime time must be after DepartureTime time.");

            if (flight.AvailableSeats > flight.TotalSeats)
                ModelState.AddModelError("AvailableSeats", "Seats available cannot exceed total seats.");

            if (!ModelState.IsValid)
                return View(flight);

            try
            {
                await _context.Flights.AddAsync(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving flight: {ex.Message}");
                return View(flight);
            }
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            return View(flight);
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Flight flight)
        {
            if (id != flight.Id) return BadRequest();

            // Custom validation
            if (flight.ArrivalTime <= flight.DepartureTime)
                ModelState.AddModelError("ArrivalTime", "ArrivalTime time must be after DepartureTime time.");

            if (flight.AvailableSeats > flight.TotalSeats)
                ModelState.AddModelError("AvailableSeats", "Seats available cannot exceed total seats.");

            if (!ModelState.IsValid)
                return View(flight);

            try
            {
                _context.Flights.Update(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(flight.Id))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating flight: {ex.Message}");
                return View(flight);
            }
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null) return NotFound();

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            try
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting flight: {ex.Message}");
                return View(flight);
            }
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(f => f.Id == id);
        }
    }
}
