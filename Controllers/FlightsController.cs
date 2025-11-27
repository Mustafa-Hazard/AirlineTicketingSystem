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

        // INDEX
        public async Task<IActionResult> Index()
        {
            var flights = await _context.Flights.ToListAsync();
            return View(flights);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flight model)
        {
            if (!ModelState.IsValid) return View(model);

            // initialize available seats from totals if not set
            model.AvailableEconomySeats = model.EconomySeats;
            model.AvailableBusinessSeats = model.BusinessSeats;
            model.AvailableFirstClassSeats = model.FirstClassSeats;

            _context.Flights.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Flight model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _context.Flights.FindAsync(model.Id);
            if (existing == null) return NotFound();

            // update fields; careful with available seats -- if total decreased below available, adjust available
            existing.FlightNumber = model.FlightNumber.Trim();
            existing.Origin = model.Origin.Trim();
            existing.Destination = model.Destination.Trim();
            existing.FromAirport = model.FromAirport?.Trim();
            existing.ToAirport = model.ToAirport?.Trim();
            existing.DepartureTime = model.DepartureTime;
            existing.ArrivalTime = model.ArrivalTime;

            // seats: adjust totals and ensure available seats are not greater than totals
            existing.EconomySeats = model.EconomySeats;
            existing.AvailableEconomySeats = Math.Min(existing.AvailableEconomySeats, model.EconomySeats);
            existing.BusinessSeats = model.BusinessSeats;
            existing.AvailableBusinessSeats = Math.Min(existing.AvailableBusinessSeats, model.BusinessSeats);
            existing.FirstClassSeats = model.FirstClassSeats;
            existing.AvailableFirstClassSeats = Math.Min(existing.AvailableFirstClassSeats, model.FirstClassSeats);

            // prices
            existing.EconomyPrice = model.EconomyPrice;
            existing.BusinessPrice = model.BusinessPrice;
            existing.FirstClassPrice = model.FirstClassPrice;

            existing.IsDelayed = model.IsDelayed;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET: Search Page
public IActionResult Search()
        {
            return View();
        }

        //POST: Search Result
[HttpPost]
public async Task<IActionResult> Search(string? flightNumber, string? origin, string? destination, DateTime? date)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrWhiteSpace(flightNumber))
                query = query.Where(f => f.FlightNumber.Contains(flightNumber));

            if (!string.IsNullOrWhiteSpace(origin))
                query = query.Where(f => f.Origin.Contains(origin));

            if (!string.IsNullOrWhiteSpace(destination))
                query = query.Where(f => f.Destination.Contains(destination));

            if (date.HasValue)
                query = query.Where(f =>
                    f.DepartureTime.Date == date.Value.Date ||
                    f.ArrivalTime.Date == date.Value.Date
                );

            var results = await query.ToListAsync();

            return View("SearchResults", results);
        }
        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();
            return View(flight);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            if (flight.Bookings != null && flight.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete flight that has bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
