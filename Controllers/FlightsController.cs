using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using AirlineTicketingSystem.Services;
using AirlineTicketingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingSystem.Controllers
{
    public class FlightsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IEmailService _emailService;

        public FlightsController(DatabaseContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        #region HELPERS

        // Populate Airports Dropdown
        private void PopulateAirports()
        {
            var airports = _context.Airports
                .OrderBy(a => a.City)
                .ToList();

            // Value: City, Text: "City - Name (IATA)"
            var list = airports.Select(a => new {
                Value = a.City,
                Text = $"{a.City} - {a.Name} ({a.IataCode})"
            });
            ViewBag.Airports = new SelectList(list, "Value", "Text");
        }

        // Validate Flight Data
        private bool ValidateFlightData(Flight model)
        {
            bool isValid = true;

            // 1) Time Validation
            if (model.ArrivalTime <= model.DepartureTime)
            {
                ModelState.AddModelError("ArrivalTime", "Arrival time must be later than departure time.");
                isValid = false;
            }

            // 2) Route Validation
            if (!string.IsNullOrWhiteSpace(model.Origin) && model.Origin == model.Destination)
            {
                ModelState.AddModelError("Destination", "Origin and Destination cannot be the same airport.");
                isValid = false;
            }

            return isValid;
        }

        // Map Flight Properties
        private void MapFlightProperties(Flight existing, Flight model)
        {
            existing.FlightNumber = model.FlightNumber.Trim();
            existing.Origin = model.Origin.Trim();
            existing.Destination = model.Destination.Trim();
            existing.FromAirport = model.Origin; // Dropdown se sync
            existing.ToAirport = model.Destination; // Dropdown se sync
            existing.DepartureTime = model.DepartureTime;
            existing.ArrivalTime = model.ArrivalTime;

            // Seat Counts & Available Seats Sync
            existing.EconomySeats = model.EconomySeats;
            existing.AvailableEconomySeats = Math.Min(existing.AvailableEconomySeats, model.EconomySeats);

            existing.BusinessSeats = model.BusinessSeats;
            existing.AvailableBusinessSeats = Math.Min(existing.AvailableBusinessSeats, model.BusinessSeats);

            existing.FirstClassSeats = model.FirstClassSeats;
            existing.AvailableFirstClassSeats = Math.Min(existing.AvailableFirstClassSeats, model.FirstClassSeats);

            // Pricing & Status
            existing.EconomyPrice = model.EconomyPrice;
            existing.BusinessPrice = model.BusinessPrice;
            existing.FirstClassPrice = model.FirstClassPrice;
            existing.IsDelayed = model.IsDelayed;
        }

        #endregion

        // ==========================================
        // PUBLIC ACTIONS
        // ==========================================

        // List of all flights (Public)
        [AllowAnonymous]
        public async Task<IActionResult> Index() => View(await _context.Flights.ToListAsync());

        // Flight Details (Public)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // Admin - Create Flight (GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateAirports();
            return View();
        }

        // Admin - Create Flight (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Flight model)
        {
            if (ModelState.IsValid && ValidateFlightData(model))
            {
                model.AvailableEconomySeats = model.EconomySeats;
                model.AvailableBusinessSeats = model.BusinessSeats;
                model.AvailableFirstClassSeats = model.FirstClassSeats;

                _context.Flights.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateAirports();
            return View(model);
        }

        // Admin - Edit Flight (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            PopulateAirports();
            return View(flight);
        }

        // Admin - Edit Flight (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Flight model)
        {
            if (ModelState.IsValid && ValidateFlightData(model))
            {
                var existing = await _context.Flights.FindAsync(model.Id);
                if (existing == null) return NotFound();

                MapFlightProperties(existing, model);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateAirports();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delay(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            return View(flight); // Ye aapki Delay.cshtml file ko load karega
        }
        // Admin - Delay Flight (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delay(int id, DateTime newDepartureTime)
        {
            // Flight ko database se dhoondain
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            // 🛑 VALIDATION: Check karein ke naya waqt scheduled waqt se pehle ka toh nahi?
            if (newDepartureTime <= flight.DepartureTime)
            {
                // "newDepartureTime" key wahi hai jo aapne View mein use ki hai
                ModelState.AddModelError("newDepartureTime", "Naya waqt scheduled departure se BAAD ka hona chahiye.");

                // Agar ghalti hai toh wapis page dikhayein bina save kiye
                return View(flight);
            }

            // Agar validation pass ho jaye toh data update karein
            flight.DepartureTime = newDepartureTime;
            flight.IsDelayed = true;

            _context.Update(flight);
            await _context.SaveChangesAsync();

            // Notification trigger karein
            await NotifyPassengersOfDelay(flight);

            TempData["Success"] = $"Flight {flight.FlightNumber} delayed successfully and passengers notified!";
            return RedirectToAction(nameof(Index));
        }

        // Notify Passengers of Delay
        private async Task NotifyPassengersOfDelay(Flight flight)
        {
            var bookings = await _context.Bookings
                .Where(b => b.FlightId == flight.Id)
                .Include(b => b.Passenger)
                .ToListAsync();

            foreach (var b in bookings)
            {
                if (b.Passenger != null)
                {
                    await _emailService.SendFlightDelayNotificationAsync(
                        b.Passenger.ContactEmail, b.Passenger.FullName,
                        flight.FlightNumber, flight.Origin, flight.Destination, flight.DepartureTime);
                }
            }
        }

        #region ADMIN ACTIONS
        // Admin: View All Flights with Bookings
        [Authorize(Roles = "Admin")]
        public IActionResult ViewAllWithBookings()
        {
            var flights = _context.Flights
                .Include(f => f.Bookings)
                    .ThenInclude(b => b.Passenger)
                .ToList();

            return View(flights);
        }
        #endregion

        // ==========================================
        // SEARCH FLIGHTS
        // ==========================================

        // Simple search by origin, destination, and date (Public)
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
    }
}
