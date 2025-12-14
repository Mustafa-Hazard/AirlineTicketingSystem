using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using AirlineTicketingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AirlineTicketingSystem.Controllers
{
    public class FlightsController : Controller
    {
        private readonly DatabaseContext _context;

        public FlightsController(DatabaseContext context)
        {
            _context = context;
        }

        // Load airports into ViewBag for dropdowns
        private void PopulateAirports()
        {
            var airports = _context.Airports
                .OrderBy(a => a.City)
                .ThenBy(a => a.Name)
                .ToList();

            // value = City, text = "City - Name (IATA)"
            ViewBag.Airports = new SelectList(airports, "City", "DisplayName");
        }

        // ========== ADMIN VIEW: FLIGHTS WITH BOOKINGS ==========

        // Admin page: each flight with its bookings (accordion style)
        [Authorize(Roles = "Admin")]
        public IActionResult ViewAllWithBookings()
        {
            var flights = _context.Flights
                .Include(f => f.Bookings)
                    .ThenInclude(b => b.Passenger)
                .ToList();

            return View(flights);
        }

        // ========== PUBLIC PAGES: LIST + DETAILS ==========

        // ✅ CUSTOMERS CAN ACCESS - Everyone can see the list of flights
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var flights = await _context.Flights.ToListAsync();
            return View(flights);
        }

        // ✅ CUSTOMERS CAN ACCESS - Everyone can see individual flight details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // ========== CREATE FLIGHT (ADMIN ONLY) ==========

        // ❌ ONLY ADMIN - GET: show empty form
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateAirports();   // needed for Origin/Destination dropdowns
            return View();
        }

        // ❌ ONLY ADMIN - POST: save new flight
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Flight model)
        {
            // Custom validation is added only if basic model binding is okay
            if (ModelState.IsValid)
            {
                // 1) Arrival must be after departure
                if (model.ArrivalTime <= model.DepartureTime)
                {
                    ModelState.AddModelError("ArrivalTime", "Arrival time must be later than departure time.");
                }

                // 2) Origin and Destination cannot be same
                if (!string.IsNullOrWhiteSpace(model.Origin) &&
                    !string.IsNullOrWhiteSpace(model.Destination) &&
                    model.Origin == model.Destination)
                {
                    ModelState.AddModelError("Destination", "Origin and Destination cannot be the same airport.");
                }
            }

            // If anything failed (data annotations or our custom checks)
            if (!ModelState.IsValid)
            {
                // repopulate dropdown before returning the view
                PopulateAirports();
                return View(model);
            }

            // Initialize available seats from total seats
            model.AvailableEconomySeats = model.EconomySeats;
            model.AvailableBusinessSeats = model.BusinessSeats;
            model.AvailableFirstClassSeats = model.FirstClassSeats;

            _context.Flights.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ========== EDIT FLIGHT (ADMIN ONLY) ==========

        // ❌ ONLY ADMIN - GET: show form with current data
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            PopulateAirports();
            return View(flight);
        }

        // ❌ ONLY ADMIN - POST: update flight
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Flight model)
        {
            // First, check built-in validation (Required, Range, etc.)
            if (ModelState.IsValid)
            {
                // 1) Arrival > Departure
                if (model.ArrivalTime <= model.DepartureTime)
                {
                    ModelState.AddModelError("ArrivalTime", "Arrival time must be later than departure time.");
                }

                // 2) Origin != Destination
                if (!string.IsNullOrWhiteSpace(model.Origin) &&
                    !string.IsNullOrWhiteSpace(model.Destination) &&
                    model.Origin == model.Destination)
                {
                    ModelState.AddModelError("Destination", "Origin and Destination cannot be the same airport.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Need dropdowns again when we re-show the form
                PopulateAirports();
                return View(model);
            }

            var existing = await _context.Flights.FindAsync(model.Id);
            if (existing == null) return NotFound();

            // Update basic fields
            existing.FlightNumber = model.FlightNumber.Trim();
            existing.Origin = model.Origin.Trim();
            existing.Destination = model.Destination.Trim();
            existing.FromAirport = model.FromAirport?.Trim();
            existing.ToAirport = model.ToAirport?.Trim();
            existing.DepartureTime = model.DepartureTime;
            existing.ArrivalTime = model.ArrivalTime;

            // Update seat counts and keep available seats in valid range
            existing.EconomySeats = model.EconomySeats;
            existing.AvailableEconomySeats = Math.Min(existing.AvailableEconomySeats, model.EconomySeats);

            existing.BusinessSeats = model.BusinessSeats;
            existing.AvailableBusinessSeats = Math.Min(existing.AvailableBusinessSeats, model.BusinessSeats);

            existing.FirstClassSeats = model.FirstClassSeats;
            existing.AvailableFirstClassSeats = Math.Min(existing.AvailableFirstClassSeats, model.FirstClassSeats);

            // Update prices and delay status
            existing.EconomyPrice = model.EconomyPrice;
            existing.BusinessPrice = model.BusinessPrice;
            existing.FirstClassPrice = model.FirstClassPrice;
            existing.IsDelayed = model.IsDelayed;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ========== SEARCH FLIGHTS (PUBLIC) ==========

        // ✅ CUSTOMERS CAN ACCESS - Simple search by origin, destination and date
        [AllowAnonymous]
        public IActionResult Search(FlightSearchVM vm)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Origin))
                query = query.Where(f => f.Origin.Contains(vm.Origin));

            if (!string.IsNullOrWhiteSpace(vm.Destination))
                query = query.Where(f => f.Destination.Contains(vm.Destination));

            if (vm.DepartureDate.HasValue)
            {
                var date = vm.DepartureDate.Value.Date;
                query = query.Where(f => f.DepartureTime.Date == date);
            }

            vm.Results = query
                .OrderBy(f => f.DepartureTime)
                .ToList();

            return View(vm);
        }

        // ========== DELETE FLIGHT (ADMIN ONLY) ==========

        // ❌ ONLY ADMIN - GET: confirm delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();
            return View(flight);
        }

        // ❌ ONLY ADMIN - POST: delete after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            // Do not allow deleting flights that already have bookings
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